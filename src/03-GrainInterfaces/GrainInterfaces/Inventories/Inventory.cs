using ProtoBuf;
using System;
using System.Collections.Generic;

namespace GrainInterfaces.Inventories
{
    [ProtoContract]
    [Serializable]
    public class Inventory
    {
        [ProtoMember(1)]
        public Guid Id;
        [ProtoMember(2)]
        public DateTimeOffset CreationDate;
        [ProtoMember(3)]
        public Guid WarehouseCode;

        /// <summary>
        /// ProductStock information for each product
        /// </summary>
        [ProtoMember(4)]
        public Dictionary<Guid, ProductStock> ProductsStocks = new Dictionary<Guid, ProductStock>();
    }
}
