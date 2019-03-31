using System;

namespace GrainInterfaces.Warehouses
{
    public class Warehouse
    {
        public Guid Id;
        public string Code;
        public string Name;
        public string Description;
        public DateTimeOffset CreationDate;
    }
}
