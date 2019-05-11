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
            Dispatched = order.Dispatched;
            Name = order.Name;
            Date = order.Date;
            TotalAmount = order.TotalAmount;
            order.Items = order.Items ?? new List<OrderItem>();
            Items = order.Items?.Select(x => new OrderItemViewModel(x)).ToList();
        }

        public Guid Id;
        public bool Dispatched;
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
    }

    public class OrderItemProductViewModel
    {
        public OrderItemProductViewModel(Product product)
        {
            if (product != null)
            {
                Id = product.Id;
                Code = product.Code;
                Description = product.Name;
                Price = product.Price;
            }
        }

        public Guid Id;
        public string Code;
        public string Description;
        public decimal Price;
    }

    public class OrderCreateRequest
    {
        public string Name;
        public List<OrderCreateItemRequest> Items;
    }

    public class OrderCreateItemRequest
    {
        public Guid ProductId;
        public decimal Quantity;
    }
}
