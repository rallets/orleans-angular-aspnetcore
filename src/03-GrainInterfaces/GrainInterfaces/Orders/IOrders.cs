using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Orders
{
    public interface IOrders : Orleans.IGrainWithGuidKey
    {
        Task<List<Order>> GetAll();
        Task<bool> Exists(Guid id);
        Task<Order> Add(Order product);
    }
}
