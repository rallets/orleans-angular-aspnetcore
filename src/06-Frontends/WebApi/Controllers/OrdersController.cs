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
        [Route("{orderGuid:Guid}/events")]
        public async Task<ActionResult<OrderEventsViewModel>> GetOrderEventsAsync(Guid orderGuid)
        {
            var exists = await _orleansClient.GetGrain<IOrders>(Guid.Empty).Exists(orderGuid);
            if(!exists)
            {
                return NotFound();
            }

            var result = await _orleansClient.GetGrain<IOrder>(orderGuid).GetEvents();
            var response = MapToViewModel(result);
            return response;
        }

        [HttpGet]
        public async Task<ActionResult<OrdersViewModel>> GetAllAsync()
        {
            var result = await _orleansClient.GetGrain<IOrders>(Guid.Empty).GetAll();
            var response = MapToViewModel(result);
            return response;
        }

        [HttpGet]
        [Route("stats")]
        public async Task<ActionResult<OrdersStatsViewModel>> GetStatsAsync()
        {
            var stats = await _orleansClient.GetGrain<IOrdersStatsCache>(Guid.Empty).GetAsync();
            var result = new OrdersStatsViewModel(stats);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<OrderViewModel>> PostAsync(Models.Orders.OrderCreateRequest request)
        {
            foreach(var item in request.Items)
            {
                var exists = await _orleansClient.GetGrain<IProduct>(item.ProductId).Created();
                if (!exists)
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
            var result = await _orleansClient.GetGrain<IOrders>(Guid.Empty).Add(order);
            var response = MapToViewModel(result);
            return response;
        }
        
    }

    public static class OrdersMapper
    {
        public static GrainInterfaces.Orders.OrderCreateRequest MapFromRequest(Models.Orders.OrderCreateRequest request)
        {
            return new GrainInterfaces.Orders.OrderCreateRequest
            {
                Items = request.Items.Select(x => MapFromRequest(x)).ToList(),
                Name = request.Name,
            };
        }

        public static OrderItemCreateRequest MapFromRequest(OrderCreateItemRequest request)
        {
            return new OrderItemCreateRequest
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
        }

        public static OrderViewModel MapToViewModel(OrderState item)
        {
            return new OrderViewModel(item);
        }

        public static OrdersViewModel MapToViewModel(IEnumerable<OrderState> items)
        {
            var result = new OrdersViewModel();
            result.Orders = items.Select(x => new OrderViewModel(x)).ToList();
            return result;
        }

        public static OrderEventsViewModel MapToViewModel(IEnumerable<OrderEventInfo> items)
        {
            var result = new OrderEventsViewModel();
            result.Events = items.Select(x => new OrderEventViewModel(x)).ToList();
            return result;
        }
    }
}
