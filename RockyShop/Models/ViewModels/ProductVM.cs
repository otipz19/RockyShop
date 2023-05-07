using Microsoft.AspNetCore.Mvc.Rendering;

namespace RockyShop.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CategoryDropDown { get; set; }
    }
}
