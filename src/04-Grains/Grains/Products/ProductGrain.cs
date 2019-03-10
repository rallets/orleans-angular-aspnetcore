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
    [StorageProvider(ProviderName = "TableStore")]
    public class ProductGrain : Grain<Product>, IProduct
    {
        private readonly ILogger _logger;

        public ProductGrain(ILogger<ProductGrain> logger)
        {
            _logger = logger;
        }

        async Task<Product> IProduct.Create(Product product)
        {
            State = product;
            await base.WriteStateAsync();
            return this.State;
        }

        Task<Product> IProduct.GetState()
        {
            return Task.FromResult(this.State);
        }
    }
}
