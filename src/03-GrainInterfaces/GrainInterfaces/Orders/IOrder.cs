using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Orders
{
    public interface IOrder : Orleans.IGrainWithGuidKey
    {
        Task<OrderState> Create(OrderCreateRequest request);
        Task<OrderState> TryDispatch(bool isNewOrder);
        Task<OrderState> GetState();
        Task<List<OrderEventInfo>> GetEvents();
    }
}
