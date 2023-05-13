using RockyShop.Model.Models;

namespace RockyShop.Model.ViewModels
{
    public class CartUserVM
    {
        public CartUserVM()
        {
            Products = new List<Product>();
        }

        public AppUser User { get; set; }

        public IList<Product> Products { get; set; }
    }
}
