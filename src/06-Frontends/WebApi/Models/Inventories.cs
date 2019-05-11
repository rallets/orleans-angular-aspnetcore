using GrainInterfaces.Inventories;
using GrainInterfaces.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models.Products;

namespace WebApi.Models.Inventories
{
    public class InventoryViewModel
    {
        public InventoryViewModel(Inventory inventory, Product[] products)
        {
            Id = inventory.Id;
            CreationDate = inventory.CreationDate;
            WarehouseCode = inventory.WarehouseCode;
            ProductsStocks = inventory.ProductsStocks
                .Select(x => new ProductStockViewModel(x.Value, products.FirstOrDefault( p=> p.Id == x.Key)))
                .ToList();
        }

        public Guid Id;
        public DateTimeOffset CreationDate;
        public Guid WarehouseCode;

        /// <summary>
        /// ProductStock information for each product
        /// </summary>
        public List<ProductStockViewModel> ProductsStocks;
    }

    public class ProductStockViewModel
    {
        public ProductStockViewModel(ProductStock stock, Product product)
        {
            if (product != null)
            {
                Product = new ProductViewModel(product);
            }
            CurrentStockQuantity = stock.CurrentStockQuantity;
            SafetyStockQuantity = stock.SafetyStockQuantity;
            BookedQuantity = stock.BookedQuantity;
            Active = stock.Active;
            StateDescription = stock.State.ToString();
        }

        public ProductViewModel Product;
        public decimal CurrentStockQuantity;
        public decimal SafetyStockQuantity;
        public decimal BookedQuantity;
        public bool Active;
        public string StateDescription;
    }
}
