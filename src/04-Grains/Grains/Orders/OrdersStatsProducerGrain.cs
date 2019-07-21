using GrainInterfaces.Inventories;
using GrainInterfaces.Models;
using GrainInterfaces.Orders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Concurrency;
using Orleans.Configuration;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansSilo.Orders
{
    [Reentrant]
    public class OrdersStatsProducerGrain : Grain, IOrdersStatsProducer
    {
        private readonly ILogger _logger;

        private readonly TimeSpan _waitTimeout;
        private VersionedValue<OrdersStats> _state;
        private TaskCompletionSource<VersionedValue<OrdersStats>> _wait;

        public OrdersStatsProducerGrain(ILogger<OrdersStatsProducerGrain> logger, IOptions<SiloMessagingOptions> messagingOptions)
        {
            _logger = logger;

            // this timeout helps resolve long polls gracefully just before orleans breaks them with a timeout exception
            // while not necessary for the reactive caching pattern to work
            // it avoid polluting the network and the logs with stack traces from timeout exceptions
            _waitTimeout = messagingOptions.Value.ResponseTimeout.Subtract(TimeSpan.FromSeconds(2));
        }

        private string GrainType => nameof(OrdersGrain);
        private string GrainKey => this.GetPrimaryKeyString();

        public override Task OnActivateAsync()
        {
            // initialize the state
            _state = VersionedValue<OrdersStats>.None.NextVersion(new OrdersStats());

            // initialize the polling wait handle
            _wait = new TaskCompletionSource<VersionedValue<OrdersStats>>();

            return base.OnActivateAsync();
        }

        public Task UpdateValue(OrdersStats value)
        {
            // update the state
            _state = _state.NextVersion(value);
            _logger.LogInformation("{@GrainType} {@GrainKey} updated value to {@Value} with version {@Version}",
                GrainType, GrainKey, _state.Value, _state.Version);

            // fulfill waiting promises
            _wait.SetResult(_state);
            _wait = new TaskCompletionSource<VersionedValue<OrdersStats>>();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns the current state without polling.
        /// </summary>
        public Task<VersionedValue<OrdersStats>> GetAsync() => Task.FromResult(_state);

        /// <summary>
        /// If the given version is the same as the current version then resolves when a new version of data is available.
        /// If no new data become available within the orleans response timeout minus some margin, then resolves with a "no data" response.
        /// Otherwise returns the current data immediately.
        /// </summary>
        public Task<VersionedValue<OrdersStats>> LongPollStatsAsync(VersionToken knownVersion) =>
            knownVersion == _state.Version
            ? _wait.Task.WithDefaultOnTimeout(_waitTimeout, VersionedValue<OrdersStats>.None)
            : Task.FromResult(_state);
    }
}
