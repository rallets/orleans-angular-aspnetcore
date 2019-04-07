using GrainInterfaces.Products;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models.Products;
using static WebApi.Controllers.ProductsMapper;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IClusterClient _orleansClient;

        public ProductsController(IClusterClient orleansClient)
        {
            _orleansClient = orleansClient;
        }

        [HttpGet]
        public async Task<ActionResult<ProductsViewModel>> GetAllAsync()
        {
            var products = _orleansClient.GetGrain<IProducts>(Guid.Empty);
            var result = await products.GetAll();
            var response = MapToViewModel(result);
            return response;
        }

        [HttpPost]
        public async Task<ActionResult<ProductViewModel>> PostAsync(ProductCreateRequest request)
        {
            var product = MapFromRequest(request);
            var products = _orleansClient.GetGrain<IProducts>(Guid.Empty);
            var result = await products.Add(product);
            var response = MapToViewModel(result);
            return response;
        }

    }

    public static class ProductsMapper
    {
        public static Product MapFromRequest(ProductCreateRequest request)
        {
            return new Product
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
            };
        }

        public static ProductViewModel MapToViewModel(Product item)
        {
            return new ProductViewModel(item);
        }

        public static ProductsViewModel MapToViewModel(IEnumerable<Product> items)
        {
            var result = new ProductsViewModel();
            result.Products = items.Select(x => new ProductViewModel(x)).ToList();
            return result;
        }
    }
}
