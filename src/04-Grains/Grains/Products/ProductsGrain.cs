using GrainInterfaces;
using GrainInterfaces.Inventories;
using GrainInterfaces.Products;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;
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
    [ImplicitStreamSubscription("ProductCreatedStream")]
    public class ProductsGrain : Grain<ProductsState>, IProducts
    {
        private readonly ILogger _logger;
        protected StreamSubscriptionHandle<Product> _sub;

        public ProductsGrain(ILogger<ProductsGrain> logger)
        {
            _logger = logger;
        }

        public async override Task OnActivateAsync()
        {
            _logger.Info($"{nameof(ProductsGrain)} => OnActivateAsync");

            var streamProvider = GetStreamProvider("AzureQueueProvider");
            var stream = streamProvider.GetStream<Product>(this.GetPrimaryKey(), "ProductCreatedStream");
            _sub = await stream.SubscribeAsync(this.OnNextAsync, this.OnErrorAsync, this.OnCompletedAsync);
            var handles = await stream.GetAllSubscriptionHandles();

            _logger.Info($"{nameof(ProductsGrain)} => Found {handles.Count} subscription handles");

            await base.OnActivateAsync();
        }

        public async Task<Product[]> GetAll()
        {
            var products = new List<Task<Product>>();
            foreach (var id in this.State.Products)
            {
                var product = GrainFactory.GetGrain<IProduct>(id);
                products.Add(product.GetState());
            }
            return await Task.WhenAll(products);
        }

        //public async Task<Product> Add(Product info)
        //{
        //    info.Id = Guid.NewGuid();
        //    info.CreationDate = DateTimeOffset.Now;

        //    var g = GrainFactory.GetGrain<IProduct>(info.Id);
        //    var product = await g.Create(info);
        //    State.Products.Add(info.Id);
        //    _logger.Info($"Product created => {info.Id}");

        //    // add the product in all inventories
        //    var gi = GrainFactory.GetGrain<IInventories>(Guid.Empty);
        //    var inventories = await gi.GetAll();
        //    foreach (var inventory in inventories)
        //    {
        //        var gx = GrainFactory.GetGrain<IInventory>(inventory.Id);
        //        await gx.AddProduct(info.Id);
        //    }

        //    await base.WriteStateAsync();
        //    return product;
        //}

        public Task<bool> Exists(Guid id)
        {
            var result = State.Products.Contains(id);
            _logger.Info($"Product exists {id} => {result}");
            return Task.FromResult(result);
        }

        public async Task OnNextAsync(Product product, StreamSequenceToken token = null)
        {
            _logger.Info($"{nameof(ProductsGrain)} => Stream OnNextAsync");

            if (State.Products.Contains(product.Id))
            {
                return;
            }

            State.Products.Add(product.Id);

            await base.WriteStateAsync();
            _logger.Info($"Product creation finalized => {product.Id}");
        }

        public Task OnCompletedAsync()
        {
            _logger.Info($"{nameof(ProductsGrain)} => Stream OnCompletedAsync");
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            _logger.Info($"{nameof(ProductsGrain)} => Stream OnErrorAsync");
            return Task.CompletedTask;
        }
    }
}
