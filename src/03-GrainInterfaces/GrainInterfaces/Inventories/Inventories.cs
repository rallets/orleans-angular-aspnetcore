using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrainInterfaces.Inventories
{
    [ProtoContract]
    [Serializable]
    public class Inventories
    {
        [ProtoMember(1)]
        public List<Guid> Items;
    }
}
