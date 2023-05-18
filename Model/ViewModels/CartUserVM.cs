using RockyShop.Model.Models;

namespace RockyShop.Model.ViewModels
{
    public class CartUserVM
    {
        public AppUser User { get; set; }

        public IList<ProductInCart> ProductInCartList { get; set; } = new List<ProductInCart>();
    }
}
