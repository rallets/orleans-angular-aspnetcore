using GrainInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Orders
{
    public interface IOrders : Orleans.IGrainWithGuidKey
    {
        Task<Order[]> GetAll();
        Task<Guid[]> GetAllNotDispatched();
        Task SetAsDispatched(Guid orderGuid);
        Task<bool> Exists(Guid id);
        Task<Order> Add(Order order);

        // Reactive pool
        Task<VersionedValue<OrdersStats>> GetStatsAsync();
        Task<VersionedValue<OrdersStats>> LongPollStatsAsync(VersionToken knownVersion);
    }
}
