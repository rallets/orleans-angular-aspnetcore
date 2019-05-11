using ProtoBuf;
using System;

namespace GrainInterfaces.Products
{
    [ProtoContract]
    [Serializable]
    public class Product
    {
        [ProtoMember(1)]
        public Guid Id;
        [ProtoMember(2)]
        public DateTimeOffset CreationDate;
        [ProtoMember(3)]
        public string Code;
        [ProtoMember(4)]
        public string Name;
        [ProtoMember(5)]
        public string Description;
        [ProtoMember(6)]
        public decimal Price;
    }
}
