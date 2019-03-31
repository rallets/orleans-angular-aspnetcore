using GrainInterfaces.Products;
using System;
using System.Collections.Generic;

namespace WebApi.Models.Products
{
    public class ProductsViewModel
    {
        public List<ProductViewModel> Products;
    }

    public class ProductViewModel
    {
        public ProductViewModel(Product product)
        {
            Id = product.Id;
            CreationDate = product.CreationDate;
            Code = product.Code;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
        }

        public Guid Id;
        public DateTimeOffset CreationDate;
        public string Code;
        public string Name;
        public string Description;
        public decimal Price;
    }

    public class ProductCreateRequest
    {
        public string Code;
        public string Name;
        public string Description;
        public decimal Price;
    }

}
