using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Warehouses
{
    public interface IWarehouse : Orleans.IGrainWithGuidKey
    {
        Task<Warehouse> Create(Warehouse Warehouse);
        Task<Warehouse> GetState();
    }
}
