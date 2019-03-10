using System;

namespace GrainInterfaces.Products
{
    public class Product
    {
        public Guid Id;
        public string Code;
        public string Name;
        public string Description;
        public DateTimeOffset CreationDate;
    }
}
