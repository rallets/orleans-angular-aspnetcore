using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Warehouses
{
    public interface IWarehouses : Orleans.IGrainWithGuidKey
    {
        Task<Warehouse[]> GetAll();
        Task<bool> Exists(Guid id);
        Task<Warehouse> Add(Warehouse Warehouse);
    }
}
