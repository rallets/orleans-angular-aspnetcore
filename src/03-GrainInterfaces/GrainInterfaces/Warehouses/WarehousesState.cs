using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrainInterfaces.Warehouses
{
    [ProtoContract]
    [Serializable]
    public class WarehousesState
    {
        [ProtoMember(1)]
        public List<Guid> Warehouses = new List<Guid>();
    }
}
