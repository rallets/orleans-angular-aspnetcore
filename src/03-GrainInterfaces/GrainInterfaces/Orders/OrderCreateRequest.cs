using GrainInterfaces.Products;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace GrainInterfaces.Orders
{
    [ProtoContract]
    [Serializable]
    public class OrderCreateRequest
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
        public List<OrderItemCreateRequest> Items;
        [ProtoMember(6)]
        public Guid? AssignedInventory;
    }

    [ProtoContract]
    [Serializable]
    public class OrderItemCreateRequest
    {
        [ProtoMember(1)]
        public Guid ProductId;
        [ProtoMember(2)]
        public Product Product;
        [ProtoMember(3)]
        public decimal Quantity;
    }
}
