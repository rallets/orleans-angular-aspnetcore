using GrainInterfaces.Inventories;
using GrainInterfaces.Orders;
using Microsoft.Extensions.Logging;
using Orleans;
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

        public OrdersGrain(ILogger<OrdersGrain> logger)
        {
            _logger = logger;
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
                await base.WriteStateAsync();
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
            await base.WriteStateAsync();

            order = await g.TryDispatch(true);
            
            return order;
        }

        public Task<bool> Exists(Guid id)
        {
            var result = State.Orders.Contains(id);
            _logger.LogInformation($"Order exists {id} => {result}");
            return Task.FromResult(result);
        }

    }
}
