using GrainInterfaces.Inventories;
using GrainInterfaces.Scheduled;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansSilo.Scheduled
{
    public class InventoryAutoSupplyingGrain : Grain, IInventoryAutoSupplying, IRemindable
    {
        private readonly ILogger _logger;
        private IGrainReminder _reminder;

        public InventoryAutoSupplyingGrain(ILogger<InventoryAutoSupplyingGrain> logger)
        {
            _logger = logger;
        }

        public async override Task OnActivateAsync()
        {
            _logger.Info($"{nameof(InventoryAutoSupplyingGrain)} => OnActivateAsync");
            _reminder = await RegisterOrUpdateReminder(nameof(InventoryAutoSupplyingGrain), TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
            await base.OnActivateAsync();
        }

        Task IInventoryAutoSupplying.Start()
        {
            // needed to awake the grain and register the reminder
            return Task.CompletedTask;
        }

        Task IInventoryAutoSupplying.Stop()
        {
            // needed to stop the reminder
            UnregisterReminder(_reminder);
            return Task.CompletedTask;
        }

        async Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        {
            _logger.Info($"Reminder {reminderName} received a reminder");

            var tasks = new List<Task<decimal>>();
            // TODO: maybe inventory can just expose a list of products that need to be replenished
            var inventories = await GrainFactory.GetGrain<IInventories>(Guid.Empty).GetAll();
            foreach (var inventory in inventories)
            {
                IInventory gi = null;

                foreach (var stock in inventory.ProductsStocks)
                {
                    var qty = stock.Value.SupplyingRequiredQuantity;
                    if (qty > 0)
                    {
                        var productGuid = stock.Key;

                        _logger.Info($"Inventory increase required for product {productGuid} for {qty} (safety stock {stock.Value.SafetyStockQuantity} + already booked {stock.Value.BookedQuantity} - currently in stock {stock.Value.CurrentStockQuantity})");

                        if (gi == null)
                        {
                            gi = GrainFactory.GetGrain<IInventory>(inventory.Id);
                        }
                        var increaseTask = gi.Increase(productGuid, qty);
                        tasks.Add(increaseTask);
                    }
                }
            }
            await Task.WhenAll(tasks);

            _logger.Info($"Reminder {reminderName} completed");
        }

    }
}
