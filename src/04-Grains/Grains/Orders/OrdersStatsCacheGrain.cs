using System;
using System.Threading.Tasks;
using GrainInterfaces.Models;
using GrainInterfaces.Orders;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;

namespace OrleansSilo
{
    [Reentrant]
    [StatelessWorker]
    public class ProducerCacheGrain : ReactiveGrain, IOrdersStatsCache
    {
        private readonly ILogger<ProducerCacheGrain> _logger;
        private VersionedValue<OrdersStats> _cache;
        private IDisposable _poll;

        public ProducerCacheGrain(ILogger<ProducerCacheGrain> logger)
        {
            _logger = logger;
        }

        private string GrainType => nameof(ProducerCacheGrain);
        private Guid GrainKey => this.GetPrimaryKey();

        public override async Task OnActivateAsync()
        {
            // start long polling
            _poll = await RegisterReactivePollAsync(
                () => GrainFactory.GetGrain<IOrders>(GrainKey).GetStatsAsync(),
                () => GrainFactory.GetGrain<IOrders>(GrainKey).LongPollStatsAsync(_cache.Version),
                result => result.IsValid,
                apply =>
                {
                    _cache = apply;
                    _logger.LogInformation(
                        "{@Time}: {@GrainType} {@GrainKey} updated value to {@Value} with version {@Version}",
                        DateTime.Now.TimeOfDay, GrainType, GrainKey, _cache.Value, _cache.Version);
                    return Task.CompletedTask;
                },
                failed =>
                {
                    _logger.LogWarning("The reactive poll timed out by returning a 'none' response before Orleans could break the promise.");
                    return Task.CompletedTask;
                });

            await base.OnActivateAsync();
        }

        public Task<OrdersStats> GetAsync() => Task.FromResult(_cache.Value);
    }
}
