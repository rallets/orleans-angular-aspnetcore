using ProtoBuf;
using System;
using System.Collections.Generic;

namespace GrainInterfaces.TestStates
{
    [ProtoContract]
    [Serializable]
    public class TestState
    {
        [ProtoMember(1)]
        public string Id;

        [ProtoMember(2)]
        public Guid AGuid;

        [ProtoMember(3)]
        public decimal ADecimal;

        [ProtoMember(4)]
        public DateTimeOffset ADateTimeOffset;

        [ProtoMember(5)]
        public Dictionary<Guid, TestState> ADictionary;
    }

}
