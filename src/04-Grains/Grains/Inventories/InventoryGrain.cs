using GrainInterfaces.Inventories;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Providers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orleans.Transactions.Abstractions;
using Orleans.Concurrency;

namespace OrleansSilo.Inventories
{
    [StorageProvider(ProviderName = "BlobStore")]
    class InventoryGrain : Grain, IInventory
    {
        private readonly ILogger _logger;
        private readonly ITransactionalState<Inventory> inventory;

        public InventoryGrain(
            ILogger<InventoryGrain> logger,
            [TransactionalState("inventory", "TransactionStore")]
            ITransactionalState<Inventory> inventory
            )
        {
            _logger = logger;
            this.inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        }

        private Guid GrainKey => this.GetPrimaryKey();

        [AlwaysInterleave]
        public Task Create(Inventory data)
        {
            return this.inventory.PerformUpdate(state =>
            {
                state.CreationDate = data.CreationDate;
                state.Id = data.Id;
                state.ProductsStocks = data.ProductsStocks;
                state.WarehouseCode = data.WarehouseCode;
                return state;
            });
        }

        [AlwaysInterleave]
        public Task AddProduct(Guid productGuid)
        {
            return this.inventory.PerformUpdate(state =>
            {
                var exists = state.ProductsStocks.Any(x => x.Key == productGuid);
                _logger.Info($"Inventory {GrainKey} - Add Product {productGuid} - Exists: {exists}");
                if (exists)
                {
                    return state;
                }

                state.ProductsStocks.Add(productGuid, new ProductStock
                {
                    CurrentStockQuantity = 0,
                    SafetyStockQuantity = 10, // TODO: should this come from the Product ?
                    BookedQuantity = 0,
                    Active = true
                });
                return state;
            });
        }

        [AlwaysInterleave]
        public Task<ProductStock> GetProductState(Guid ProductGuid)
        {
            return this.inventory.PerformRead(state =>
            {
                var stockState = state.ProductsStocks.FirstOrDefault(x => x.Key == ProductGuid).Value;
                return stockState;
            });
        }

        [AlwaysInterleave]
        public Task Increase(Guid productGuid, decimal quantity)
        {
            return this.inventory.PerformUpdate(state =>
            {
                var productStock = state.ProductsStocks.First(x => x.Key == productGuid).Value;

                var currentStockQuantity = productStock.CurrentStockQuantity;
                var bookedQuantity = productStock.BookedQuantity;

                if (bookedQuantity > 0)
                {
                    if (quantity >= bookedQuantity)
                    {
                        productStock.CurrentStockQuantity += quantity;
                        productStock.BookedQuantity = 0;
                    }
                    else
                    {
                        productStock.CurrentStockQuantity += quantity;
                        productStock.BookedQuantity -= quantity;
                    }
                }
                else
                {
                    productStock.CurrentStockQuantity += quantity;
                }

                state.ProductsStocks.First(x => x.Key == productGuid).Value.CurrentStockQuantity = productStock.CurrentStockQuantity;
                state.ProductsStocks.First(x => x.Key == productGuid).Value.BookedQuantity = productStock.BookedQuantity;

                _logger.Info($"Inventory {GrainKey} - Product {productGuid} increased from {currentStockQuantity}/{bookedQuantity} to {productStock.CurrentStockQuantity}/{productStock.BookedQuantity}");

                return state;
            });
        }

        [AlwaysInterleave]
        public Task Deduct(Guid productGuid, decimal quantity)
        {
            return this.inventory.PerformUpdate(state =>
            {
                var productStock = state.ProductsStocks.First(x => x.Key == productGuid).Value;
                var currentStockQuantity = productStock.CurrentStockQuantity;
                var bookedQuantity = productStock.BookedQuantity;

                if (currentStockQuantity >= quantity)
                {
                    // order can be dispatched
                    state.ProductsStocks.First(x => x.Key == productGuid).Value.CurrentStockQuantity -= quantity;
                    _logger.Info($"Inventory {GrainKey} - Product {productGuid} deducted CurrentStock {currentStockQuantity} of {quantity}");
                    return state;
                }

                // order cannot be dispatched, add in the booked queue
                state.ProductsStocks.First(x => x.Key == productGuid).Value.BookedQuantity += quantity;
                _logger.Info($"Inventory {GrainKey} - Product {productGuid} increased BookedQuantity {bookedQuantity} of {quantity}");
                return state;
            });
        }

        [AlwaysInterleave]
        public Task<Inventory> GetState()
        {
            return this.inventory.PerformRead(state =>
            {
                return state;
            });
        }

    }
}
