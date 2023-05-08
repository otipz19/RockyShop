using Microsoft.AspNetCore.Mvc.Rendering;

namespace RockyShop.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; } = new Product();

        public IEnumerable<SelectListItem> CategoryDropDown { get; set; }

        public IEnumerable<SelectListItem> AppTypeDropDown { get; set; }
    }
}
