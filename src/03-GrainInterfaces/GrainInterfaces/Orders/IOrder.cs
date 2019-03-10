using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Orders
{
    public interface IOrder : Orleans.IGrainWithGuidKey
    {
        Task<Order> Create(Order product);
        Task<Order> GetState();
    }
}
