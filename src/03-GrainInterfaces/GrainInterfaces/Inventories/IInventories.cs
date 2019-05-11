using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Inventories
{
    public interface IInventories : Orleans.IGrainWithGuidKey
    {
        Task<Inventory[]> GetAll();
        Task<Guid> GetBestForProduct(Guid productGuid);
        Task<Inventory> Get(Guid warehouseCode);
        Task<bool> Exists(Guid warehouseCode);
        Task<Inventory> Add(Inventory inventory);
    }
}
