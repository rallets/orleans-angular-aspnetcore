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
            Code = product.Code;
            Name = product.Name;
            Description = product.Description;
            CreationDate = product.CreationDate;
        }

        public Guid Id;
        public string Code;
        public string Name;
        public string Description;
        public DateTimeOffset CreationDate;
    }

    public class ProductCreateRequest
    {
        public string Code;
        public string Name;
        public string Description;
    }

}
