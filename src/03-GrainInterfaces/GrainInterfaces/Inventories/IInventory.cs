using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces.Inventories
{
    public interface IInventory : Orleans.IGrainWithGuidKey
    {
        Task<Inventory> Create(Inventory inventory);
        Task<Inventory> GetState();
        Task AddProduct(Guid productGuid);
        Task<ProductStock> GetProductState(Guid productGuid);
        Task<decimal> Increase(Guid productGuid, decimal quantity);
        Task<decimal> Deduct(Guid productGuid, decimal quantity);
    }

    public interface IInventoryQueryable : Orleans.IGrainWithGuidKey
    {
        Task<Dictionary<Guid, StockState>> GetAvailable();
        Task<Dictionary<Guid, StockState>> GetInSafetyStockQuantity();
    }
}
