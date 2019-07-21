using GrainInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Orders
{
    public interface IOrdersStatsProducer : Orleans.IGrainWithGuidKey
    {
        Task UpdateValue(OrdersStats value);

        // Reactive pool
        Task<VersionedValue<OrdersStats>> GetAsync();
        Task<VersionedValue<OrdersStats>> LongPollStatsAsync(VersionToken knownVersion);
    }
}
