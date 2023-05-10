namespace RockyShop.Models.ViewModels
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
