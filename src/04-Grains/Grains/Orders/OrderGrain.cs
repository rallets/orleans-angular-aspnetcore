using GrainInterfaces.Orders;
using GrainInterfaces.Products;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
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
            var products = await GetProductsAsync(Order.Items);
            foreach (var item in Order.Items)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
            }
            Order.TotalAmount = Order.Items.Sum(item => item.Quantity * item.Product.Price);
            State = Order;
            await base.WriteStateAsync();
            return this.State;
        }

        private async Task<Product[]> GetProductsAsync(List<OrderItem> items)
        {
            var products = new List<Task<Product>>();
            foreach (var item in items)
            {
                var product = GrainFactory.GetGrain<IProduct>(item.ProductId);
                products.Add(product.GetState());
            }
            return await Task.WhenAll(products);
        }

        Task<Order> IOrder.GetState()
        {
            return Task.FromResult(this.State);
        }
    }
}
