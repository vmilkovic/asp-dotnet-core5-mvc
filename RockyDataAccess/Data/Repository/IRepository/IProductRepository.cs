using Microsoft.AspNetCore.Mvc.Rendering;
using RockyModels;
using System.Collections.Generic;

namespace RockyDataAccess.Data.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);

        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}
