using GrainInterfaces;
using GrainInterfaces.Products;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using GrainInterfaces.Inventories;

namespace OrleansSilo.Products
{
    [StorageProvider(ProviderName = "TableStore")]
    public class ProductGrain : Grain<Product>, IProduct
    {
        private readonly ILogger _logger;

        public ProductGrain(ILogger<ProductGrain> logger)
        {
            _logger = logger;
        }

        public Task<bool> Created() => Task.FromResult(State?.CreationDate != DateTimeOffset.MinValue);

        public async Task<Product> Create(Product product)
        {
            product.Id = this.GetPrimaryKey();

            var streamProvider = GetStreamProvider("AzureQueueProvider");
            var stream = streamProvider.GetStream<Product>(Guid.Empty, "ProductCreatedStream");

            // NOTE: In SMS task completes when the onNextAsync completes on the receiver. 
            //       In azure queue this will return as soon the msg in queued.
            await stream.OnNextAsync(product);
            // TODO: manage error handling: if the Task fails, retry

            // add the product in all inventories
            // TODO: move this to a stream: every inventory can listen to the ProductCreatedStream and add the product itself
            //       in this way it should be faster too
            var gi = GrainFactory.GetGrain<IInventories>(Guid.Empty);
            var inventories = await gi.GetAll();
            foreach (var inventoryGuid in inventories)
            {
                var gx = GrainFactory.GetGrain<IInventory>(inventoryGuid);
                await gx.AddProduct(product.Id);
            }

            State = product;
            await base.WriteStateAsync();

            _logger.Info($"Product created => {product.Id}");

            return State;
        }

        public Task<Product> GetState()
        {
            return Task.FromResult(State);
        }
    }
}
