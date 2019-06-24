using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.Orders;
using GrainInterfaces.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using WebApi.Models.Orders;
using static WebApi.Controllers.OrdersMapper;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IClusterClient _orleansClient;

        public OrdersController(IClusterClient orleansClient)
        {
            _orleansClient = orleansClient;
        }

        [HttpGet]
        public async Task<ActionResult<OrdersViewModel>> GetAllAsync()
        {
            var Orders = _orleansClient.GetGrain<IOrders>(Guid.Empty);
            var result = await Orders.GetAll();
            var response = MapToViewModel(result);
            return response;
        }

        [HttpGet]
        [Route("stats")]
        public async Task<ActionResult<OrdersStatsViewModel>> GetStatsAsync()
        {
            var statsCache = _orleansClient.GetGrain<IOrdersStatsCache>(Guid.Empty);
            var stats = await statsCache.GetAsync();
            var result = new OrdersStatsViewModel(stats);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<OrderViewModel>> PostAsync(OrderCreateRequest request)
        {
            foreach(var item in request.Items)
            {
                var products = _orleansClient.GetGrain<IProducts>(Guid.Empty);
                var exists = await products.Exists(item.ProductId);
                if(!exists)
                {
                    return NotFound();
                }

                if(item.Quantity < 0)
                {
                    ModelState.AddModelError(nameof(item.Quantity), "Invalid");
                    return BadRequest(ModelState);
                }
            }

            var order = MapFromRequest(request);
            var orders = _orleansClient.GetGrain<IOrders>(Guid.Empty);
            var result = await orders.Add(order);
            var response = MapToViewModel(result);
            return response;
        }
        
    }

    public static class OrdersMapper
    {
        public static Order MapFromRequest(OrderCreateRequest request)
        {
            return new Order
            {
                Items = request.Items.Select(x => MapFromRequest(x)).ToList(),
                Name = request.Name,
            };
        }

        public static OrderItem MapFromRequest(OrderCreateItemRequest request)
        {
            return new OrderItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
        }

        public static OrderViewModel MapToViewModel(Order item)
        {
            return new OrderViewModel(item);
        }

        public static OrdersViewModel MapToViewModel(IEnumerable<Order> items)
        {
            var result = new OrdersViewModel();
            result.Orders = items.Select(x => new OrderViewModel(x)).ToList();
            return result;
        }
    }
}
