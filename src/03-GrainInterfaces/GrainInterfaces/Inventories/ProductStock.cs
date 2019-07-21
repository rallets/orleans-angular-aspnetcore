using ProtoBuf;
using System;

namespace GrainInterfaces.Inventories
{
    public enum StockState
    {
        /// <summary>
        /// An unknown state, it cannot happen (assertion error)
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Current stock quantity greather than 0 (order can be processed)
        /// </summary>
        Available,

        /// <summary>
        /// Current stock quantity less than 0 (order cannot be processed)
        /// </summary>
        Unavailable,

        /// <summary>
        /// No longer active, it cannot be used or processed
        /// </summary>
        OutOfStock
    }

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
        // [ProtoMember(5), DefaultValue(StockState.Unknown)]
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
                else // if(CurrentStockQuantity <= 0)
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
                    return SafetyStockQuantity + BookedQuantity;
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
