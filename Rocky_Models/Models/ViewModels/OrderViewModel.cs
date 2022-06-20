using RockyModels.Models;
using System.Collections.Generic;

namespace RockyModels.ViewModels
{
    public class OrderViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
