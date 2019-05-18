using GrainInterfaces.Inventories;
using GrainInterfaces.Orders;
using GrainInterfaces.Scheduled;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansSilo.Scheduled
{
    public class OrderScheduledProcessingGrain : Grain, IOrderScheduledProcessing, IRemindable
    {
        private readonly ILogger _logger;
        private IGrainReminder _reminder;

        public OrderScheduledProcessingGrain(ILogger<OrderScheduledProcessingGrain> logger)
        {
            _logger = logger;
        }

        public async override Task OnActivateAsync()
        {
            _logger.Info($"{nameof(OrderScheduledProcessingGrain)} => OnActivateAsync");
            _reminder = await RegisterOrUpdateReminder(nameof(OrderScheduledProcessingGrain), TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
            await base.OnActivateAsync();
        }

        Task IOrderScheduledProcessing.Start()
        {
            // needed to awake the grain and register the reminder
            return Task.CompletedTask;
        }

        Task IOrderScheduledProcessing.Stop()
        {
            // needed to stop the reminder
            UnregisterReminder(_reminder);
            return Task.CompletedTask;
        }

        async Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        {
            _logger.Info($"Reminder {reminderName} received a reminder");

            var tasks = new List<Task<Order>>();
            var g = GrainFactory.GetGrain<IOrders>(Guid.Empty);
            var orders = await g.GetAllNotDispatched();

            var i = 0;
            foreach (var orderGuid in orders)
            {
                i++;
                _logger.Info($"Order try-to-dispatch required for order {orderGuid} {i}/{orders.Length}");

                var go = GrainFactory.GetGrain<IOrder>(orderGuid);
                var tryTask = go.TryDispatch(false);
                tasks.Add(tryTask);
            }
            await Task.WhenAll(tasks);

            _logger.Info($"Reminder {reminderName} completed");
        }

    }
}
