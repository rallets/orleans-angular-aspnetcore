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
    public class OrdersStatsCacheGrain : ReactiveGrain, IOrdersStatsCache
    {
        private readonly ILogger<OrdersStatsCacheGrain> _logger;
        private VersionedValue<OrdersStats> _cache;
        private IDisposable _poll;

        public OrdersStatsCacheGrain(ILogger<OrdersStatsCacheGrain> logger)
        {
            _logger = logger;
        }

        private string GrainType => nameof(OrdersStatsCacheGrain);
        private Guid GrainKey => this.GetPrimaryKey();

        public override async Task OnActivateAsync()
        {
            // start long polling
            _poll = await RegisterReactivePollAsync(
                () => GrainFactory.GetGrain<IOrdersStatsProducer>(GrainKey).GetAsync(),
                () => GrainFactory.GetGrain<IOrdersStatsProducer>(GrainKey).LongPollStatsAsync(_cache.Version),
                result => result.IsValid,
                apply =>
                {
                    _cache = apply;
                    _logger.LogInformation(
                        "{@Time}: {@GrainType} {@GrainKey} updated value to {@Value}",
                        DateTime.Now.TimeOfDay, GrainType, GrainKey, _cache.Value);
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
