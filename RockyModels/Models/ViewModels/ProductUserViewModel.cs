using System.Collections.Generic;

namespace RockyModels.ViewModels
{
    public class ProductUserViewModel
    {
        public ProductUserViewModel()
        {
            ProductList = new List<Product>();
        }

        public ApplicationUser ApplicationUser { get; set; }

        public List<Product> ProductList { get; set; }
    }
}
