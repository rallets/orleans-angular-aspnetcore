using System;
using System.Collections.Generic;

namespace GrainInterfaces.Orders
{
    public class Orders
    {
        public List<Guid> Items;
        public DateTimeOffset LastUpdateDate;
    }
}
