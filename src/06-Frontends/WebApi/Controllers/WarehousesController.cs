using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.Warehouses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using WebApi.Models.Warehouses;
using static WebApi.Controllers.WarehousesMapper;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private IClusterClient _orleansClient;

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
            var result = new WarehousesViewModel();
            result.Warehouses = items.Select(x => new WarehouseViewModel(x)).ToList();
            return result;
        }
    }
}
