using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Products
{
    public interface IProducts : Orleans.IGrainWithGuidKey
    {
        Task<Product[]> GetAll();
        Task<bool> Exists(Guid id);
        //Task<Product> Add(Product product);
    }
}
