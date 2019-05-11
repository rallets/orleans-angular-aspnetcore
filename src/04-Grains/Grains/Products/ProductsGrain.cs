using GrainInterfaces;
using GrainInterfaces.Inventories;
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

        async Task<Product[]> IProducts.GetAll()
        {
            var products = new List<Task<Product>>();
            foreach (var id in this.State.Products)
            {
                var product = GrainFactory.GetGrain<IProduct>(id);
                products.Add(product.GetState());
            }
            return await Task.WhenAll(products);
        }

        async Task<Product> IProducts.Add(Product info)
        {
            info.Id = Guid.NewGuid();
            info.CreationDate = DateTimeOffset.Now;

            var g = GrainFactory.GetGrain<IProduct>(info.Id);
            var product = await g.Create(info);
            State.Products.Add(info.Id);
            _logger.LogInformation($"Product created => {info.Id}");

            // add the product in all inventories
            var gi = GrainFactory.GetGrain<IInventories>(Guid.Empty);
            var inventories = await gi.GetAll();
            foreach(var inventory in inventories)
            {
                var gx = GrainFactory.GetGrain<IInventory>(inventory.Id);
                await gx.AddProduct(info.Id);
            }

            await base.WriteStateAsync();
            return product;
        }

        Task<bool> IProducts.Exists(Guid id)
        {
            var result = State.Products.Contains(id);
            _logger.LogInformation($"Product exists {id} => {result}");
            return Task.FromResult(result);
        }

    }
}
