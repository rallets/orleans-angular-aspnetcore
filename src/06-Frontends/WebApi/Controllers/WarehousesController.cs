using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.Inventories;
using GrainInterfaces.Products;
using GrainInterfaces.Warehouses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using WebApi.Models.Inventories;
using WebApi.Models.Warehouses;
using static WebApi.Controllers.WarehousesMapper;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IClusterClient _orleansClient;

        public WarehousesController(IClusterClient orleansClient)
        {
            _orleansClient = orleansClient;
        }

        [HttpGet]
        public async Task<ActionResult<WarehousesViewModel>> GetAllAsync()
        {
            var Warehouses = _orleansClient.GetGrain<IWarehouses>(Guid.Empty);
            var result = await Warehouses.GetAll();
            var response = MapToViewModel(result);
            return response;
        }

        [HttpPost]
        public async Task<ActionResult<WarehouseViewModel>> PostAsync(WarehouseCreateRequest request)
        {
            var Warehouse = MapFromRequest(request);
            var Warehouses = _orleansClient.GetGrain<IWarehouses>(Guid.Empty);
            var result = await Warehouses.Add(Warehouse);
            var response = MapToViewModel(result);
            return response;
        }

        [HttpGet]
        [Route("{warehouseGuid:guid}/inventory/")]
        public async Task<ActionResult<InventoryViewModel>> GetInventoryAsync(Guid warehouseGuid)
        {
            var gi = _orleansClient.GetGrain<IInventories>(Guid.Empty);
            var exists = await gi.ExistsWarehouse(warehouseGuid);
            if(!exists)
            {
                return NotFound();
            }

            var inventoryTask = gi.GetFromWarehouse(warehouseGuid);

            var gp = _orleansClient.GetGrain<IProducts>(Guid.Empty);
            var productsTask = gp.GetAll();

            var inventory = await inventoryTask;
            var products = await productsTask;

            var response = MapToViewModel(inventory, products);
            return response;
        }
    }

    public static class WarehousesMapper
    {
        public static Warehouse MapFromRequest(WarehouseCreateRequest request)
        {
            return new Warehouse
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
            };
        }

        public static WarehouseViewModel MapToViewModel(Warehouse item)
        {
            return new WarehouseViewModel(item);
        }

        public static WarehousesViewModel MapToViewModel(IEnumerable<Warehouse> items)
        {
            var result = new WarehousesViewModel
            {
                Warehouses = items.Select(x => new WarehouseViewModel(x)).ToList()
            };
            return result;
        }

        public static InventoryViewModel MapToViewModel(Inventory item, Product[] products)
        {
            return new InventoryViewModel(item, products);
        }
    }
}
