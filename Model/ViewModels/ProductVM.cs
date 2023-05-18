using Microsoft.AspNetCore.Mvc.Rendering;
using RockyShop.Model.Models;

namespace RockyShop.Model.ViewModels
{
    public class ProductVM
    {
        public Models.Product Product { get; set; } = new Models.Product();

        public IEnumerable<SelectListItem> CategoryDropDown { get; set; }

        public IEnumerable<SelectListItem> AppTypeDropDown { get; set; }
    }
}
