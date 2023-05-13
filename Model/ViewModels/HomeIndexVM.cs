using RockyShop.Model.Models;

namespace RockyShop.Model.ViewModels
{
    public class HomeIndexVM
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}
