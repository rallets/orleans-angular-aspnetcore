using GrainInterfaces;
using GrainInterfaces.Products;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansSilo.Products
{
    public class ProductsState
    {
        public List<Guid> Products = new List<Guid>();
    }

    [StorageProvider(ProviderName = "BlobStore")]
    public class ProductsGrain : Grain<ProductsState>, IProducts
    {
        private readonly ILogger _logger;
        

        public ProductsGrain(ILogger<ProductsGrain> logger)
        {
            _logger = logger;
        }

        async Task<List<Product>> IProducts.GetAll()
        {
            var result = new List<Product>();
            foreach (var id in this.State.Products)
            {
                var product = GrainFactory.GetGrain<IProduct>(id);
                var state = await product.GetState();
                
                result.Add(state);
            }
            return result;
        }

        async Task<Product> IProducts.Add(Product info)
        {
            info.Id = Guid.NewGuid();
            info.CreationDate = DateTimeOffset.Now;
            var product = GrainFactory.GetGrain<IProduct>(info.Id);
            var result = await product.Create(info);
            State.Products.Add(info.Id);
            _logger.LogInformation($"Product created => {info.Id}");
            await base.WriteStateAsync();
            return result;
        }

        Task<bool> IProducts.Exists(Guid id)
        {
            var result = State.Products.Contains(id);
            _logger.LogInformation($"Product exists {id} => {result}");
            return Task.FromResult(result);
        }

    }
}
