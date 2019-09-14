using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces.Inventories
{
    public interface IInventory : Orleans.IGrainWithGuidKey
    {
        [Transaction(TransactionOption.CreateOrJoin)]
        Task Create(Inventory inventory);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<Inventory> GetState();

        [Transaction(TransactionOption.CreateOrJoin)]
        Task AddProduct(Guid productGuid);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<ProductStock> GetProductState(Guid productGuid);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task Increase(Guid productGuid, decimal quantity);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task Deduct(Guid productGuid, decimal quantity);
    }
}
