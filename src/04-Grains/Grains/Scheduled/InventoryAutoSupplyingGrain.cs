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

            // TODO: maybe inventory can just expose a list of products that need to be replenished
            var inventories = await GrainFactory.GetGrain<IInventories>(Guid.Empty).GetAll();
            foreach (var inventoryGuid in inventories)
            {
                IInventory gi = GrainFactory.GetGrain<IInventory>(inventoryGuid);
                var state = await gi.GetState();

                foreach (var stock in state.ProductsStocks)
                {
                    var qty = stock.Value.SupplyingRequiredQuantity;
                    if (qty > 0)
                    {
                        var productGuid = stock.Key;

                        _logger.Info($"Inventory increase required for product {productGuid} for {qty} (safety stock {stock.Value.SafetyStockQuantity} + already booked {stock.Value.BookedQuantity} - currently in stock {stock.Value.CurrentStockQuantity})");

                        await gi.Increase(productGuid, qty);
                        var currentStockRemaining = (await gi.GetProductState(productGuid)).CurrentStockQuantity;
                    }
                }
            }

            _logger.Info($"Reminder {reminderName} completed");
        }

    }
}
