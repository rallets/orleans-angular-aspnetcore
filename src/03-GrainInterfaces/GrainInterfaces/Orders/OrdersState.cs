using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrainInterfaces.Orders
{
    [ProtoContract]
    [Serializable]
    public class OrdersState
    {
        [ProtoMember(1)]
        public List<Guid> Orders = new List<Guid>();
        [ProtoMember(2)]
        public List<Guid> OrdersNotDispatched = new List<Guid>(); // TODO: can state be splitted in 2 interfaces to reduce lock surface?
    }
}
