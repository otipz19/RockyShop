using Microsoft.AspNetCore.Mvc.Rendering;
using RockyShop.Model.Enums;
using RockyShop.Model.Models;

namespace RockyShop.Model.ViewModels
{
    public class OrderIndexVM
    {
        public IEnumerable<OrderHeader> OrderHeaders { get; set; }

        public IEnumerable<SelectListItem> StatusSelectList { get; set; }

        public OrderStatus ChosenStatus { get; set; }
    }
}
