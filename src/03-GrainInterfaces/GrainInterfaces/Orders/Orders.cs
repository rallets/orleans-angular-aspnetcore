using System;
using System.Collections.Generic;

namespace GrainInterfaces.Orders
{
    public class Orders
    {
        public List<Guid> Items;
        public DateTimeOffset LastUpdateDate;
    }

    public class OrdersStats
    {
        public int Orders = 0;
        public int OrdersNotDispatched = 0;
    }
}
