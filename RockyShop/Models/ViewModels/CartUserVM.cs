namespace RockyShop.Models.ViewModels
{
    public class CartUserVM
    {
        public CartUserVM()
        {
            Products = new List<Product>();
        }

        public AppUser User { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
