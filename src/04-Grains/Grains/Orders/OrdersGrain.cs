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
    public class OrdersState
    {
        public List<Guid> Orders = new List<Guid>();
        public List<Guid> OrdersNotDispatched = new List<Guid>(); // TODO: can state be splitted in 2 interfaces to reduce lock surface?
    }

    [StorageProvider(ProviderName = "BlobStore")]
    [Reentrant]
    public class OrdersGrain : Grain<OrdersState>, IOrders
    {
        private readonly ILogger _logger;

        private readonly TimeSpan _waitTimeout;
        private VersionedValue<OrdersStats> _state;
        private TaskCompletionSource<VersionedValue<OrdersStats>> _wait;

        public OrdersGrain(ILogger<OrdersGrain> logger, IOptions<SiloMessagingOptions> messagingOptions)
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

        protected override async Task WriteStateAsync()
        {
            await UpdateStatsAsync();

            await base.WriteStateAsync();
        }

        /// <summary>
        /// Returns the current state without polling.
        /// </summary>
        public Task<VersionedValue<OrdersStats>> GetStatsAsync() => Task.FromResult(_state);

        /// <summary>
        /// If the given version is the same as the current version then resolves when a new version of data is available.
        /// If no new data become available within the orleans response timeout minus some margin, then resolves with a "no data" response.
        /// Otherwise returns the current data immediately.
        /// </summary>
        public Task<VersionedValue<OrdersStats>> LongPollStatsAsync(VersionToken knownVersion) =>
            knownVersion == _state.Version
            ? _wait.Task.WithDefaultOnTimeout(_waitTimeout, VersionedValue<OrdersStats>.None)
            : Task.FromResult(_state);

        public async Task<Order[]> GetAll()
        {
            var orders = new List<Task<Order>>();
            foreach (var id in this.State.Orders)
            {
                var order = GrainFactory.GetGrain<IOrder>(id);
                orders.Add(order.GetState());
            }
            return await Task.WhenAll(orders);
        }

        public Task<Guid[]> GetAllNotDispatched()
        {
            return Task.FromResult(this.State.OrdersNotDispatched.ToArray());
        }

        public async Task SetAsDispatched(Guid orderGuid)
        {
            _logger.LogInformation($"Order set as dispatched and removed from list => Id: {orderGuid}");

            if (State.OrdersNotDispatched.Contains(orderGuid))
            {
                State.OrdersNotDispatched.Remove(orderGuid);
                await WriteStateAsync();
            }
        }

        public async Task<Order> Add(Order info)
        {
            info.Id = Guid.NewGuid();
            info.Date = DateTimeOffset.Now;
            var g = GrainFactory.GetGrain<IOrder>(info.Id);
            var order = await g.Create(info);
            _logger.LogInformation($"Order created => Id: {order.Id} Name: {order.Name} Dispatched: {order.Dispatched}");

            State.OrdersNotDispatched.Add(order.Id);
            State.Orders.Add(order.Id);
            await WriteStateAsync();

            order = await g.TryDispatch(true);

            return order;
        }

        public Task<bool> Exists(Guid id)
        {
            var result = State.Orders.Contains(id);
            _logger.LogInformation($"Order exists {id} => {result}");
            return Task.FromResult(result);
        }

        private Task UpdateStatsAsync()
        {
            int orders = State.Orders.Count;
            int ordersNotDipatched = State.OrdersNotDispatched.Count;

            // update the state
            _state = _state.NextVersion(new OrdersStats { Orders = orders, OrdersNotDispatched = ordersNotDipatched });
            _logger.LogInformation("{@GrainType} {@GrainKey} updated value to {@Value} with version {@Version}",
                GrainType, GrainKey, _state.Value, _state.Version); // TODO: check logs

            // fulfill waiting promises
            _wait.SetResult(_state);
            _wait = new TaskCompletionSource<VersionedValue<OrdersStats>>();

            return Task.CompletedTask;
        }
    }
}
