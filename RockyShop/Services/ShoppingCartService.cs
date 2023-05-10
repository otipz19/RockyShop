using RockyShop.Data;
using RockyShop.Models;
using RockyShop.Utilities;

namespace RockyShop.Services
{
    public class ShoppingCartService
    {
        private const string ShoppingCartSessionKey = "ShoppingCartSession";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;

        private ShoppingCart _cart;

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _cart = GetFromSession();
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
            SaveToSession();
        }

        public void RemoveFromCart(int productId)
        {
            _cart.ProductsId.Remove(productId);
            SaveToSession();
        }


        public void ClearCart()
        {
            _cart.ProductsId.Clear();
            SaveToSession();
        }

        public IList<Product> GetProductsFromCart()
        {
            return _dbContext.Products.Where(p => _cart.ProductsId.Contains(p.Id)).ToList();
        }

        private ShoppingCart GetFromSession()
        {
            ShoppingCart cart = _httpContextAccessor.HttpContext.Session.Get<ShoppingCart>(ShoppingCartSessionKey);
            return cart ?? new ShoppingCart();
        }

        private void SaveToSession()
        {
            if (_cart == null)
                throw new ApplicationException();
            _httpContextAccessor.HttpContext.Session.Set(ShoppingCartSessionKey, _cart);
        }
    }
}
