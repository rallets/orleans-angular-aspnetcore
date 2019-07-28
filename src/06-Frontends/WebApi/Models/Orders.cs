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
        public OrderViewModel(OrderState order)
        {
            Id = order.Id;
            Dispatched = order.DispatchedDate.HasValue;
            Name = order.Name;
            Date = order.Date;
            TotalAmount = order.TotalAmount;
            order.Items = order.Items ?? new List<OrderStateItem>();
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
        public OrderItemViewModel(OrderStateItem item)
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

    public class OrdersStatsViewModel
    {
        public OrdersStatsViewModel(OrdersStats stats)
        {
            All = stats.Orders;
            NotDispatched = stats.OrdersNotDispatched;
        }

        public int All;
        public int NotDispatched;
    }

    public class OrderEventsViewModel
    {
        public List<OrderEventViewModel> Events;
    }

    public class OrderEventViewModel
    {
        public OrderEventViewModel(OrderEventInfo info)
        {
            Name = info.Name;
            Date = info.Date;
        }

        public string Name;
        public DateTimeOffset Date;
    }
}
