using System;
using System.Collections.Generic;
using System.Text;

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
}
