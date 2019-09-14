using GrainInterfaces;
using GrainInterfaces.Warehouses;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
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

        public async Task<Warehouse> Create(Warehouse warehouse)
        {
            State = warehouse;
            await base.WriteStateAsync();

            _logger.Info($"{nameof(Warehouse)} created => {warehouse.Id}");
            return State;
        }

        public Task<Warehouse> GetState()
        {
            return Task.FromResult(State);
        }
    }
}
