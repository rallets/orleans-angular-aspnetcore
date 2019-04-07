using System;

namespace GrainInterfaces.Products
{
    public class Product
    {
        public Guid Id;

        public DateTimeOffset CreationDate;

        public string Code;

        public string Name;

        public string Description;

        public decimal Price;
    }
}
