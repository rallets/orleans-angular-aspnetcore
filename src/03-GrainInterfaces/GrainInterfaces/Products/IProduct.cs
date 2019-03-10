using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Products
{
    public interface IProduct : Orleans.IGrainWithGuidKey
    {
        Task<Product> Create(Product product);
        Task<Product> GetState();
    }
}
