using GrainInterfaces.Orders;
using GrainInterfaces.Products;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainInterfaces.Orders
{
    public abstract class OrderEvent
    {
        [ProtoMember(1)]
        public DateTimeOffset Date { get; set; }
    }

    [ProtoContract]
    [Serializable]
    public class OrderCreatedEvent : OrderEvent
    {
        public OrderCreatedEvent() { }
        public OrderCreatedEvent(OrderCreateRequest order)
        {
            Date = order.Date;
            Id = order.Id;
            Name = order.Name;
            TotalAmount = order.TotalAmount;
            Items = order.Items.Select(x => new OrderStateItem(x)).ToList();
        }

        [ProtoMember(2)]
        public Guid Id;
        [ProtoMember(3)]
        public string Name;
        [ProtoMember(4)]
        public decimal TotalAmount;
        [ProtoMember(5)]
        public List<OrderStateItem> Items;
        [ProtoMember(6)]
        public Guid? AssignedInventory = null;
    }

    [ProtoContract]
    [Serializable]
    public class OrderDispatchedEvent : OrderEvent { }

    [ProtoContract]
    [Serializable]
    public class OrderInventoryAssignedEvent : OrderEvent
    {
        [ProtoMember(2)]
        public Guid InventoryId;
    }

    [ProtoContract]
    [Serializable]
    public class OrderInventoryUnassignedEvent : OrderEvent { }

    [ProtoContract]
    [Serializable]
    public class OrderNotProcessableEvent : OrderEvent { }

    [ProtoContract]
    [Serializable]
    public class OrderEventInfo
    {
        public OrderEventInfo() { }
        public OrderEventInfo(OrderEvent @event)
        {
            Name = @event.GetType().Name;
            Date = @event.Date;
        }

        [ProtoMember(2)]
        public string Name;
        [ProtoMember(3)]
        public DateTimeOffset Date;
        [ProtoMember(4)]
        public string Description;
    }

}
