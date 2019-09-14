using GrainInterfaces.Orders;
using GrainInterfaces.Products;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainInterfaces.Orders
{
    [ProtoContract]
    [Serializable]
    public class OrderState
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
        public List<OrderStateItem> Items;
        [ProtoMember(6)]
        public DateTimeOffset? DispatchedDate = null;
        [ProtoMember(7)]
        public Guid? AssignedInventoryId = null;

        public OrderState Apply(OrderCreatedEvent @event)
        {
            Id = @event.Id;
            Date = @event.Date;
            Name = @event.Name;
            TotalAmount = @event.TotalAmount;
            Items = @event.Items;

            DispatchedDate = null;
            AssignedInventoryId = @event.AssignedInventory;

            return this;
        }

        public OrderState Apply(OrderDispatchedEvent @event)
        {
            DispatchedDate = @event.Date;
            return this;
        }

        public OrderState Apply(OrderInventoryAssignedEvent @event)
        {
            AssignedInventoryId = @event.InventoryId;
            return this;
        }

        public OrderState Apply(OrderInventoryUnassignedEvent @event)
        {
            AssignedInventoryId = null;
            return this;
        }

        public OrderState Apply(OrderNotProcessableEvent @event)
        {
            return this;
        }

    }

    [ProtoContract]
    [Serializable]
    public class OrderStateItem
    {
        public OrderStateItem() { }

        public OrderStateItem(OrderItemCreateRequest item)
        {
            ProductId = item.ProductId;
            Product = item.Product;
            Quantity = item.Quantity;
        }

        [ProtoMember(1)]
        public Guid ProductId;
        [ProtoMember(2)]
        public Product Product;
        [ProtoMember(3)]
        public decimal Quantity;
    }

}
