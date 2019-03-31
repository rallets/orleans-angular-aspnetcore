using GrainInterfaces;
using GrainInterfaces.Warehouses;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansSilo.Warehouses
{
    [StorageProvider(ProviderName = "TableStore")]
    public class WarehouseGrain : Grain<Warehouse>, IWarehouse
    {
        private readonly ILogger _logger;

        public WarehouseGrain(ILogger<WarehouseGrain> logger)
        {
            _logger = logger;
        }

        async Task<Warehouse> IWarehouse.Create(Warehouse Warehouse)
        {
            State = Warehouse;
            await base.WriteStateAsync();
            return State;
        }

        Task<Warehouse> IWarehouse.GetState()
        {
            return Task.FromResult(State);
        }
    }
}
