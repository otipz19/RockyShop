using RockyShop.Data;
using RockyShop.Models;
using RockyShop.Utilities;

namespace RockyShop.Services
{
    public class ShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;

        private ShoppingCart _cart;

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _cart = GetShoppingCart();
        }

        public int CartCount()
        {
            return _cart.ProductsId.Count();
        }

        public bool CartContains(int productId)
        {
            return _cart.ProductsId.Contains(productId);
        }

        public void AddToCart(int productId)
        {
            _cart.ProductsId.Add(productId);
            SaveShoppingCart();
        }

        public void RemoveFromCart(int productId)
        {
            _cart.ProductsId.Remove(productId);
            SaveShoppingCart();
        }

        public IEnumerable<Product> GetProductsFromCart()
        {
            return _dbContext.Products.Where(p => _cart.ProductsId.Contains(p.Id));
        }

        private ShoppingCart GetShoppingCart()
        {
            ShoppingCart cart = _httpContextAccessor.HttpContext.Session.Get<ShoppingCart>(Constants.ShoppingCartSessionKey);
            return cart ?? new ShoppingCart();
        }

        private void SaveShoppingCart()
        {
            if (_cart == null)
                throw new ApplicationException();
            _httpContextAccessor.HttpContext.Session.Set(Constants.ShoppingCartSessionKey, _cart);
        }
    }
}
