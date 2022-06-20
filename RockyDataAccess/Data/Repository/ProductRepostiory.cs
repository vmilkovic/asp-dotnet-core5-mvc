using Microsoft.AspNetCore.Mvc.Rendering;
using RockyDataAccess.Data.Repository.IRepository;
using RockyModels;
using RockyUtility;
using System.Collections.Generic;
using System.Linq;

namespace RockyDataAccess.Data.Repository
{
    public class ProductRepistory : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepistory(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == WC.CategoryName)
            {
                return _db.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }

            if (obj == WC.ApplicationTypeName)
            {
                return _db.ApplicationTypes.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }

            return null;
        }

        public void Update(Product product)
        {
            _db.Products.Update(product);
        }
    }
}
