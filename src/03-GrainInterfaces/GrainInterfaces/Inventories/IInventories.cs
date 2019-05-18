using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Inventories
{
    public interface IInventories : Orleans.IGrainWithGuidKey
    {
        Task<Inventory[]> GetAll();
        Task<bool> Exists(Guid inventoryGuid);
        Task<Inventory> Add(Inventory inventory);
        Task<Guid> GetBestForProduct(Guid productGuid);
        Task<Inventory> GetFromWarehouse(Guid warehouseCode);
        Task<bool> ExistsWarehouse(Guid warehouseCode);
    }
}
