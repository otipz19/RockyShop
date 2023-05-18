using Microsoft.AspNetCore.Http;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using RockyShop.Model.ViewModels;
using RockyShop.Utility.Utilities;

namespace RockyShop.Utility.Services
{
    public class ShoppingCartService
    {
        private const string ShoppingCartSessionKey = "ShoppingCartSession";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepo;

        private ShoppingCart _cart;

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor, IProductRepository productRepo)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepo = productRepo;
            _cart = GetFromSession();
        }

        public bool IsCartFromInquiry => _cart.InquiryId != 0;

        public int InquiryId
        {
            get => _cart.InquiryId;
            set
            {
                _cart.InquiryId = value;
                SaveToSession();
            }
        }

        public int CartCount()
        {
            return _cart.Items.Count();
        }

        public bool CartContains(int productId)
        {
            return _cart.Items.Select(i => i.ProductId).Contains(productId);
        }

        public void AddToCart(int productId, int sqFt)
        {
            _cart.Items.Add(new ShoppingCartItem()
            {
                ProductId = productId,
                SqFt = sqFt,
            });
            SaveToSession();
        }

        public void AddToCartRange(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                _cart.Items.Add(new ShoppingCartItem()
                {
                    ProductId = product.Id,
                });
            }
            SaveToSession();
        }

        public void AddToCartRange(IEnumerable<ProductInCart> productInCartList)
        {
            foreach (var productInCart in productInCartList)
            {
                _cart.Items.Add(new ShoppingCartItem()
                {
                    ProductId = productInCart.Product.Id,
                    SqFt = productInCart.SqFt,
                });
            }
            SaveToSession();
        }

        public void RemoveFromCart(int productId)
        {
            ShoppingCartItem toRemove = _cart.Items.FirstOrDefault(i => i.ProductId == productId);
            _cart.Items.Remove(toRemove);
            SaveToSession();
        }

        public void ClearCartItems()
        {
            _cart.Items.Clear();
            SaveToSession();
        }

        public void ResetCart()
        {
            _cart = new ShoppingCart();
            SaveToSession();
        }

        public IList<Product> GetProducts()
        {
            List<int> productIdList = _cart.Items.Select(i => i.ProductId).ToList();
            return _productRepo.GetAll(p => productIdList.Contains(p.Id)).ToList(); 
        }

        public IList<ProductInCart> GetProductsInCart()
        {
            var products = GetProducts();
            var productInCartList = new List<ProductInCart>();
            foreach(var product in products)
            {
                productInCartList.Add(new ProductInCart()
                {
                    Product = product,
                    SqFt = _cart.Items.First(i => i.ProductId == product.Id).SqFt,
                });
            }
            return productInCartList;
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
