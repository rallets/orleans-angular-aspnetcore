using GrainInterfaces.Warehouses;
using System;
using System.Collections.Generic;

namespace WebApi.Models.Warehouses
{
    public class WarehousesViewModel
    {
        public List<WarehouseViewModel> Warehouses;
    }

    public class WarehouseViewModel
    {
        public WarehouseViewModel(Warehouse Warehouse)
        {
            Id = Warehouse.Id;
            Code = Warehouse.Code;
            Name = Warehouse.Name;
            Description = Warehouse.Description;
            CreationDate = Warehouse.CreationDate;
        }

        public Guid Id;
        public string Code;
        public string Name;
        public string Description;
        public DateTimeOffset CreationDate;
    }

    public class WarehouseCreateRequest
    {
        public string Code;
        public string Name;
        public string Description;
    }

}
