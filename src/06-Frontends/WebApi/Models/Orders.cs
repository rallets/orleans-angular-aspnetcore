using GrainInterfaces.Orders;
using GrainInterfaces.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Models.Orders
{
    public class OrdersViewModel
    {
        public List<OrderViewModel> Orders;
    }

    public class OrderViewModel
    {
        public OrderViewModel(Order order)
        {
            Id = order.Id;
            Name = order.Name;
            Date = order.Date;
            TotalAmount = order.TotalAmount;
            Items = order.Items.Select(x => new OrderItemViewModel(x)).ToList();
        }

        public Guid Id;
        public string Name;
        public DateTimeOffset Date;
        public decimal TotalAmount;
        public List<OrderItemViewModel> Items;
    }

    public class OrderItemViewModel
    {
        public OrderItemViewModel(OrderItem item)
        {
            Product = new OrderItemProductViewModel(item.Product);
            Quantity = item.Quantity;
        }

        public OrderItemProductViewModel Product;
        public decimal Quantity;
        // public decimal UnitPrice;
    }

    public class OrderItemProductViewModel
    {
        public OrderItemProductViewModel(Product product)
        {
            if (product != null)
            {
                Id = product.Id;
                Description = product.Name;
            }
        }

        public Guid Id;
        public string Description;
    }

    public class OrderCreateRequest
    {
        public decimal TotalAmount;
        public string Name;
        public List<OrderCreateItemRequest> Items;
    }

    public class OrderCreateItemRequest
    {
        public Guid ProductId;
        public decimal Quantity;
    }
}
