using GrainInterfaces.Orders;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansSilo.Orders
{
    public class OrdersState
    {
        public List<Guid> Orders = new List<Guid>();
    }

    [StorageProvider(ProviderName = "BlobStore")]
    public class OrdersGrain : Grain<OrdersState>, IOrders
    {
        private readonly ILogger _logger;

        public OrdersGrain(ILogger<OrdersGrain> logger)
        {
            _logger = logger;
        }

        async Task<Order[]> IOrders.GetAll()
        {
            var orders = new List<Task<Order>>();
            foreach (var id in this.State.Orders)
            {
                var order = GrainFactory.GetGrain<IOrder>(id);
                orders.Add(order.GetState());
            }
            return await Task.WhenAll(orders);
        }

        async Task<Order> IOrders.Add(Order info)
        {
            info.Id = Guid.NewGuid();
            info.Date = DateTimeOffset.Now;
            var Order = GrainFactory.GetGrain<IOrder>(info.Id);
            var result = await Order.Create(info);
            State.Orders.Add(info.Id);
            _logger.LogInformation($"Order created => {info.Id}");
            await base.WriteStateAsync();
            return result;
        }

        Task<bool> IOrders.Exists(Guid id)
        {
            var result = State.Orders.Contains(id);
            _logger.LogInformation($"Order exists {id} => {result}");
            return Task.FromResult(result);
        }

    }
}
