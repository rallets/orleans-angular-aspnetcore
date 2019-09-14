using GrainInterfaces.Inventories;
using GrainInterfaces.Orders;
using GrainInterfaces.Products;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans.EventSourcing;

namespace OrleansSilo.Orders
{
    [LogConsistencyProvider(ProviderName = "LogStorage")]
    [StorageProvider(ProviderName = "BlobStore")]
    public class OrderGrain : JournaledGrain<OrderState, OrderEvent>, IOrder
    {
        private readonly ILogger _logger;

        public OrderGrain(ILogger<OrderGrain> logger)
        {
            _logger = logger;
        }

        public async Task<OrderState> Create(OrderCreateRequest order)
        {
            var products = await GetProductsAsync(order.Items.Select(x => x.ProductId).Distinct().ToList());
            foreach (var item in order.Items)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
            }
            order.TotalAmount = order.Items.Sum(item => item.Quantity * item.Product.Price);

            RaiseEvent(new OrderCreatedEvent(order));
            await ConfirmEvents();
            return State;
        }

        public async Task<OrderState> TryDispatch(bool isNewOrder)
        {
            _logger.Info($"Order try to dispatch - Id: {State.Id} New: {isNewOrder}");

            var gis = GrainFactory.GetGrain<IInventories>(Guid.Empty);

            if (State.AssignedInventoryId.HasValue && !await gis.Exists(State.AssignedInventoryId.Value))
            {
                RaiseEvent(new OrderInventoryUnassignedEvent { Date = DateTimeOffset.Now });
                await ConfirmEvents();
            }

            if (!State.AssignedInventoryId.HasValue)
            {
                // find best inventory and store it, booked quantity is for inventory
                var inventoryId = await gis.GetBestForProduct(State.Items.First().ProductId);
                RaiseEvent(new OrderInventoryAssignedEvent {
                    Date = DateTimeOffset.Now,
                    InventoryId = inventoryId
                });
                await ConfirmEvents();
            }

            var gi = GrainFactory.GetGrain<IInventory>(State.AssignedInventoryId.Value);

            // try to dispatch all items, otherwise set order as "not processable"
            var isOrderProcessable = true;
            foreach (var item in State.Items)
            {
                if(item.ProductId == Guid.Empty)
                {
                    string e = "Invalid guid";
                }

                if (isNewOrder)
                {
                    await gi.Deduct(item.ProductId, item.Quantity);
                    // var produckStockRemaining = await gi.Deduct(item.ProductId, item.Quantity);
                    var produckStockRemaining = (await gi.GetProductState(item.ProductId)).CurrentStockQuantity;
                    var isItemProcessable = (produckStockRemaining >= item.Quantity);
                    if (!isItemProcessable)
                    {
                        // TODO: send event for non-processable order item
                    }
                    isOrderProcessable &= isItemProcessable;
                }
                else
                {
                    var s = await gi.GetProductState(item.ProductId);
                    if (s.CurrentStockQuantity < item.Quantity)
                    {
                        isOrderProcessable = false;
                        break;
                    }
                }
            }
            if (!isNewOrder && isOrderProcessable)
            {
                foreach (var item in State.Items)
                {
                    // var produckStockRemaining = await gi.Deduct(item.ProductId, item.Quantity);
                    await gi.Deduct(item.ProductId, item.Quantity);
                    var produckStockRemaining = (await gi.GetProductState(item.ProductId)).CurrentStockQuantity;
                    var isItemProcessable = (produckStockRemaining >= 0);
                    isOrderProcessable &= isItemProcessable;
                }
            }

            if (isOrderProcessable)
            {
                // TODO: send event for processable order
            }

            if (isOrderProcessable)
            {
                RaiseEvent(new OrderDispatchedEvent {
                    Date = DateTimeOffset.Now
                });
                await ConfirmEvents();

                 _logger.Info($"Order {State.Id} set as dispatched");
                 var gorders = GrainFactory.GetGrain<IOrders>(Guid.Empty);
                 await gorders.SetAsDispatched(State.Id);
            }
            else
            {
                RaiseEvent(new OrderNotProcessableEvent {
                    Date = DateTimeOffset.Now
                });
                await ConfirmEvents();

                _logger.Info($"Order not processable - Id: {State.Id} New: {isNewOrder}");
            }
            return State;
        }

        private async Task<Product[]> GetProductsAsync(List<Guid> items)
        {
            var products = new List<Task<Product>>();
            foreach (var item in items)
            {
                var task = GrainFactory.GetGrain<IProduct>(item).GetState();
                products.Add(task);
            }
            return await Task.WhenAll(products);
        }

        public Task<OrderState> GetState()
        {
            return Task.FromResult(this.State);
        }

        public async Task<List<OrderEventInfo>> GetEvents()
        {
            var events = await RetrieveConfirmedEvents(0, Version);
            var result = events.Select(x => new OrderEventInfo(x as OrderEvent)).ToList();
            return result;
        }
    }

}
