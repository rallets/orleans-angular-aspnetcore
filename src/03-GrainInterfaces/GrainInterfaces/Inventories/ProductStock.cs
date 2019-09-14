using ProtoBuf;
using System;

namespace GrainInterfaces.Inventories
{
    [ProtoContract]
    [Serializable]
    public class ProductStock
    {
        [ProtoMember(1)]
        public decimal CurrentStockQuantity;
        [ProtoMember(2)]
        public decimal SafetyStockQuantity;
        [ProtoMember(3)]
        public decimal BookedQuantity;
        [ProtoMember(4)]
        public bool Active;

        public StockState State
        {
            get
            {
                if (!Active)
                {
                    return StockState.OutOfStock;
                }
                else if (CurrentStockQuantity > 0)
                {
                    return StockState.Available;
                }
                else
                {
                    return StockState.Unavailable;
                }
            }
        }

        public decimal SupplyingRequiredQuantity
        {
            get
            {
                if(!Active)
                {
                    return 0;
                }
                if (BookedQuantity > 0) {
                    return (SafetyStockQuantity - CurrentStockQuantity) + BookedQuantity;
                }
                if(CurrentStockQuantity < SafetyStockQuantity)
                {
                    return SafetyStockQuantity - CurrentStockQuantity;
                }
                return 0;
            }
        }
    }
}
