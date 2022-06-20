using Microsoft.AspNetCore.Mvc.Rendering;
using RockyModels.Models;
using System.Collections.Generic;

namespace RockyModels.ViewModels
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderHeader> OrderHeaderList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string Status { get; set; }
    }
}
