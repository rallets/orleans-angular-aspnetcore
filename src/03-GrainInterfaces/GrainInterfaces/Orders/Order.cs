using GrainInterfaces.Products;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace GrainInterfaces.Orders
{
    [ProtoContract]
    [Serializable]
    public class Order
    {
        [ProtoMember(1)]
        public Guid Id;
        [ProtoMember(2)]
        public DateTimeOffset Date;
        [ProtoMember(3)]
        public string Name;
        [ProtoMember(4)]
        public decimal TotalAmount;
        [ProtoMember(5)]
        public List<OrderItem> Items;
        [ProtoMember(6)]
        public bool Dispatched;
        [ProtoMember(7)]
        public Guid AssignedInventory = Guid.Empty;

    }

    [ProtoContract]
    [Serializable]
    public class OrderItem
    {
        [ProtoMember(1)]
        public Guid ProductId;
        [ProtoMember(2)]
        public Product Product;
        [ProtoMember(3)]
        public decimal Quantity;
    }
}
