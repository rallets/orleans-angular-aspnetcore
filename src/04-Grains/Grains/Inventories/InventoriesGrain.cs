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

        async Task<Inventory[]> IInventories.GetAll()
        {
            var inventories = new List<Task<Inventory>>();
            foreach (var kvp in this.State.Inventories)
            {
                var inventory = GrainFactory.GetGrain<IInventory>(kvp.Key);
                inventories.Add(inventory.GetState());
            }
            return await Task.WhenAll(inventories);
        }

        async Task<Guid> IInventories.GetBestForProduct(Guid productGuid)
        {
            (Guid inventoryGuid, decimal qty) result = (Guid.Empty, decimal.MinValue);
            
            foreach (var kvp in this.State.Inventories)
            {
                var g = GrainFactory.GetGrain<IInventory>(kvp.Key);
                var stockState = await g.GetProductState(productGuid);
                var stockQty = stockState.CurrentStockQuantity - stockState.BookedQuantity;
                if (stockQty > result.qty)
                {
                    result = (kvp.Key, stockQty);
                }
            }

            return result.inventoryGuid;
        }

        async Task<Inventory> IInventories.Add(Inventory data)
        {
            data.Id = Guid.NewGuid();
            data.CreationDate = DateTimeOffset.Now;
            var inventory = GrainFactory.GetGrain<IInventory>(data.Id);
            var result = await inventory.Create(data);
            State.Inventories.Add(data.Id, data.WarehouseCode);
            _logger.LogInformation($"Inventory created => {data.Id} {data.WarehouseCode}");
            await base.WriteStateAsync();
            return result;
        }

        Task<bool> IInventories.Exists(Guid warehouseGuid)
        {
            var result = State.Inventories.Values.Any(x => x == warehouseGuid);
            _logger.LogInformation($"Inventory with warehouse id {warehouseGuid} exists => {result}");
            return Task.FromResult(result);
        }

        async Task<Inventory> IInventories.Get(Guid warehouseGuid)
        {
            var id = State.Inventories.First(x => x.Value == warehouseGuid).Key;
            var g = GrainFactory.GetGrain<IInventory>(id);
            var state = await g.GetState();
            return state;
        }
    }
}
