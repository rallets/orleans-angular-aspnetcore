using GrainInterfaces.Inventories;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansSilo.Inventories
{
    [StorageProvider(ProviderName = "BlobStore")]
    class InventoryGrain : Grain<Inventory>, IInventory
    {
        private readonly ILogger _logger;

        public InventoryGrain(ILogger<InventoryGrain> logger)
        {
            _logger = logger;
        }

        async Task<Inventory> IInventory.Create(Inventory data)
        {
            State = data;
            await base.WriteStateAsync();
            return State;
        }

        async Task IInventory.AddProduct(Guid productGuid)
        {
            var exists = State.ProductsStocks.Any(x => x.Key == productGuid);
            if(exists)
            {
                return;
            }

            State.ProductsStocks.Add(productGuid, new ProductStock
            {
                CurrentStockQuantity = 0,
                SafetyStockQuantity = 10, // TODO: should this come from the Product ?
                BookedQuantity = 0,
                Active = true
            });
            
            await base.WriteStateAsync();
        }

        Task<ProductStock> IInventory.GetProductState(Guid ProductGuid)
        {
            var stockState = State.ProductsStocks.First(x => x.Key == ProductGuid).Value;
            return Task.FromResult(stockState);
        }

        async Task<decimal> IInventory.Increase(Guid productGuid, decimal quantity)
        {
            var productStock = State.ProductsStocks.First(x => x.Key == productGuid).Value;
            if (productStock.BookedQuantity > 0)
            {
                if (quantity >= productStock.BookedQuantity)
                {
                    productStock.CurrentStockQuantity += quantity;
                    productStock.BookedQuantity = 0;
                } else
                {
                    productStock.CurrentStockQuantity += quantity;
                    productStock.BookedQuantity -= quantity;
                }
            }
            else
            {
                productStock.CurrentStockQuantity += quantity;
            }
            await base.WriteStateAsync();
            return productStock.CurrentStockQuantity;
        }

        async Task<decimal> IInventory.Deduct(Guid productGuid, decimal quantity)
        {
            var productStock = State.ProductsStocks.First(x => x.Key == productGuid).Value;

            if (productStock.CurrentStockQuantity >= quantity)
            {
                // order can be dispatched
                productStock.CurrentStockQuantity -= quantity;
                await base.WriteStateAsync();
                return productStock.CurrentStockQuantity;
            }

            // order cannot be dispatched, add in the booked queue
            productStock.BookedQuantity += quantity;
            await base.WriteStateAsync();
            return -productStock.BookedQuantity;
        }

        Task<Inventory> IInventory.GetState()
        {
            return Task.FromResult(this.State);
        }

    }
}
