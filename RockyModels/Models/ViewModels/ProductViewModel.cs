using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RockyModels.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CategorySelectList { get; set; }

        public IEnumerable<SelectListItem> ApplicationTypeSelectList { get; set; }
    }
}
