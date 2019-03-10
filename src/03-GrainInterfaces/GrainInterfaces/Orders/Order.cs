using GrainInterfaces.Products;
using System;
using System.Collections.Generic;

namespace GrainInterfaces.Orders
{
    public class Order
    {
        public Guid Id;
        public string Name;
        public DateTimeOffset Date;
        public decimal TotalAmount;
        public List<OrderItem> Items;
    }

    public class OrderItem
    {
        public Guid ProductId;
        public Product Product;
        public decimal Quantity;
    }
}
