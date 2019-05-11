using GrainInterfaces;
using GrainInterfaces.Inventories;
using GrainInterfaces.Products;
using GrainInterfaces.Warehouses;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansSilo.Warehouses
{
    public class WarehousesState
    {
        public List<Guid> Warehouses = new List<Guid>();
    }

    [StorageProvider(ProviderName = "BlobStore")]
    public class WarehousesGrain : Grain<WarehousesState>, IWarehouses
    {
        private readonly ILogger _logger;

        public WarehousesGrain(ILogger<WarehousesGrain> logger)
        {
            _logger = logger;
        }

        async Task<Warehouse[]> IWarehouses.GetAll()
        {
            var warehouses = new List<Task<Warehouse>>();

            foreach (var id in this.State.Warehouses)
            {
                var warehouse = GrainFactory.GetGrain<IWarehouse>(id);
                warehouses.Add(warehouse.GetState());
            }

            return await Task.WhenAll(warehouses);
        }

        async Task<Warehouse> IWarehouses.Add(Warehouse info)
        {
            info.Id = Guid.NewGuid();
            info.CreationDate = DateTimeOffset.Now;
            var Warehouse = GrainFactory.GetGrain<IWarehouse>(info.Id);
            var result = await Warehouse.Create(info);
            State.Warehouses.Add(info.Id);
            _logger.LogInformation($"Warehouse created => {info.Id}");

            // add all products in the new inventory
            var gp = GrainFactory.GetGrain<IProducts>(Guid.Empty);
            var products = await gp.GetAll();
            var stocks = products.ToDictionary(x => x.Id, x => new ProductStock
            {
                Active = true,
                BookedQuantity = 0,
                CurrentStockQuantity = 0,
                SafetyStockQuantity = 10, // TODO: should this come from the Product ?
            });
            var inventory = new Inventory
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.Now,
                ProductsStocks = stocks,
                WarehouseCode = info.Id
            };

            var gi = GrainFactory.GetGrain<IInventories>(Guid.Empty);
            var inventories = await gi.Add(inventory);

            await base.WriteStateAsync();
            return result;
        }

        Task<bool> IWarehouses.Exists(Guid id)
        {
            var result = State.Warehouses.Contains(id);
            _logger.LogInformation($"Warehouse exists {id} => {result}");
            return Task.FromResult(result);
        }

    }
}
