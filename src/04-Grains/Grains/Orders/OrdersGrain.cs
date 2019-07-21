using GrainInterfaces.Inventories;
using GrainInterfaces.Models;
using GrainInterfaces.Orders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Concurrency;
using Orleans.Configuration;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansSilo.Orders
{
    public class OrdersState
    {
        public List<Guid> Orders = new List<Guid>();
        public List<Guid> OrdersNotDispatched = new List<Guid>(); // TODO: can state be splitted in 2 interfaces to reduce lock surface?
    }

    [StorageProvider(ProviderName = "BlobStore")]
    public class OrdersGrain : Grain<OrdersState>, IOrders
    {
        private readonly ILogger _logger;

        public OrdersGrain(ILogger<OrdersGrain> logger, IOptions<SiloMessagingOptions> messagingOptions)
        {
            _logger = logger;
        }

        private string GrainType => nameof(OrdersGrain);
        private string GrainKey => this.GetPrimaryKeyString();

        public override async Task OnActivateAsync()
        {
            await this.UpdateStatsAsync();            

            await base.OnActivateAsync();
        }

        protected override async Task WriteStateAsync()
        {
            await UpdateStatsAsync();

            await base.WriteStateAsync();
        }

        public async Task<Order[]> GetAll()
        {
            var orders = new List<Task<Order>>();
            foreach (var id in this.State.Orders)
            {
                var order = GrainFactory.GetGrain<IOrder>(id);
                orders.Add(order.GetState());
            }
            return await Task.WhenAll(orders);
        }

        // [AlwaysInterleave]
        public Task<Guid[]> GetAllNotDispatched()
        {
            return Task.FromResult(this.State.OrdersNotDispatched.ToArray());
        }

        public async Task SetAsDispatched(Guid orderGuid)
        {
            _logger.LogInformation($"Order set as dispatched and removed from list => Id: {orderGuid}");

            if (State.OrdersNotDispatched.Contains(orderGuid))
            {
                State.OrdersNotDispatched.Remove(orderGuid);
                await WriteStateAsync();
            }
        }

        public async Task<Order> Add(Order info)
        {
            info.Id = Guid.NewGuid();
            info.Date = DateTimeOffset.Now;
            var g = GrainFactory.GetGrain<IOrder>(info.Id);
            var order = await g.Create(info);
            _logger.LogInformation($"Order created => Id: {order.Id} Name: {order.Name} Dispatched: {order.Dispatched}");

            State.OrdersNotDispatched.Add(order.Id);
            State.Orders.Add(order.Id);
            await WriteStateAsync();

            order = await g.TryDispatch(true);

            return order;
        }

        public Task<bool> Exists(Guid id)
        {
            var result = State.Orders.Contains(id);
            _logger.LogInformation($"Order exists {id} => {result}");
            return Task.FromResult(result);
        }

        private async Task UpdateStatsAsync()
        {
            int orders = State.Orders.Count;
            int ordersNotDipatched = State.OrdersNotDispatched.Count;

            // update the stats state
            var value = new OrdersStats { Orders = orders, OrdersNotDispatched = ordersNotDipatched };
            _logger.LogInformation("{@GrainType} {@GrainKey} updating value to {@Value}", GrainType, GrainKey, value); // TODO: check logs

            await GrainFactory.GetGrain<IOrdersStatsProducer>(Guid.Empty).UpdateValue(value);
        }
    }
}
