using GrainInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Orders
{
    public interface IOrders : Orleans.IGrainWithGuidKey
    {
        Task<OrderState[]> GetAll();
        Task<Guid[]> GetNotDispatched();
        Task SetAsDispatched(Guid orderGuid);
        Task<bool> Exists(Guid id);
        Task<OrderState> Add(OrderCreateRequest request);
    }
}
