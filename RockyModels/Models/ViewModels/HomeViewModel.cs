using System.Collections.Generic;

namespace RockyModels.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Category> Categories { get; set;}
    }
}
