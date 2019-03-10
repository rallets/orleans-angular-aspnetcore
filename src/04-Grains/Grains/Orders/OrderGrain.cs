using GrainInterfaces.Orders;
using GrainInterfaces.Products;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System.Threading.Tasks;

namespace OrleansSilo.Orders
{
    [StorageProvider(ProviderName = "TableStore")]
    public class OrderGrain : Grain<Order>, IOrder
    {
        private readonly ILogger _logger;

        public OrderGrain(ILogger<OrderGrain> logger)
        {
            _logger = logger;
        }

        async Task<Order> IOrder.Create(Order Order)
        {
            foreach(var item in Order.Items)
            {
                var product = GrainFactory.GetGrain<IProduct>(item.ProductId);
                var state = await product.GetState();
                item.Product = state;
            }
            State = Order;
            await base.WriteStateAsync();
            return this.State;
        }

        Task<Order> IOrder.GetState()
        {
            return Task.FromResult(this.State);
        }
    }
}
