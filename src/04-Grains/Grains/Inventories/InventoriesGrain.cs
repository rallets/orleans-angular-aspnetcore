using GrainInterfaces.Inventories;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansSilo.Inventories
{
    public class InventoriesState
    {
        /// <summary>
        /// InventoryGuid, WarehouseGuid
        /// </summary>
        public Dictionary<Guid, Guid> Inventories = new Dictionary<Guid, Guid>();
    }

    [StorageProvider(ProviderName = "BlobStore")]
    public class InventoriesGrain : Grain<InventoriesState>, IInventories
    {
        private readonly ILogger _logger;

        public InventoriesGrain(ILogger<InventoriesGrain> logger)
        {
            _logger = logger;
        }

        public Task<Guid[]> GetAll()
        {
            var ids = this.State.Inventories.Select(x => x.Key).ToArray();
            return Task.FromResult(ids);
            //var inventories = new List<Task<Inventory>>();
            //foreach (var kvp in this.State.Inventories)
            //{
            //    var inventory = GrainFactory.GetGrain<IInventory>(kvp.Key);
            //    inventories.Add(inventory.GetState());
            //}
            //return await Task.WhenAll(inventories);
        }

        public async Task<Guid> GetBestForProduct(Guid productGuid)
        {
            (Guid inventoryGuid, decimal qty) result = (Guid.Empty, decimal.MinValue);
            
            foreach (var kvp in this.State.Inventories)
            {
                var g = GrainFactory.GetGrain<IInventory>(kvp.Key);
                var stockState = await g.GetProductState(productGuid);
                if (stockState == null)
                {
                    continue;
                }

                var stockQty = stockState.CurrentStockQuantity - stockState.BookedQuantity;
                if (stockQty > result.qty)
                {
                    result = (kvp.Key, stockQty);
                }
            }

            return result.inventoryGuid;
        }

        public async Task<Inventory> Add(Inventory data)
        {
            data.Id = Guid.NewGuid();
            data.CreationDate = DateTimeOffset.Now;
            var inventory = GrainFactory.GetGrain<IInventory>(data.Id);
            await inventory.Create(data);
            var result = await inventory.GetState();
            State.Inventories.Add(data.Id, data.WarehouseCode);
            _logger.LogInformation($"Inventory created => {data.Id} {data.WarehouseCode}");
            await base.WriteStateAsync();
            return result;
        }

        public Task<bool> Exists(Guid inventoryGuid)
        {
            var result = State.Inventories.Any(x => x.Key == inventoryGuid);
            _logger.LogInformation($"Inventory with id {inventoryGuid} exists => {result}");
            return Task.FromResult(result);
        }

        public Task<bool> ExistsWarehouse(Guid warehouseGuid)
        {
            var result = State.Inventories.Values.Any(x => x == warehouseGuid);
            _logger.LogInformation($"Inventory with warehouse id {warehouseGuid} exists => {result}");
            return Task.FromResult(result);
        }

        public async Task<Inventory> GetByWarehouse(Guid warehouseGuid)
        {
            var id = State.Inventories.First(x => x.Value == warehouseGuid).Key;
            var g = GrainFactory.GetGrain<IInventory>(id);
            var state = await g.GetState();
            return state;
        }
    }
}
