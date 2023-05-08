using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockyShop.Data;
using RockyShop.Models;
using RockyShop.Models.ViewModels;
using System.Diagnostics;
using RockyShop.Utilities;

namespace RockyShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeIndexVM()
            {
                Products = _dbContext.Products
                    .Include(p => p.Category)
                    .Include(p => p.ApplicationType),
                Categories = _dbContext.Categories
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            ShoppingCart shoppingCart = GetShoppingCart();
            var viewModel = new HomeDetailsVM()
            {
                Product = _dbContext.Products
                    .Include(p => p.Category)
                    .Include(p => p.ApplicationType)
                    .Where(p => p.Id == id)
                    .FirstOrDefault(),
                ExistsInCart = shoppingCart?.ProductsId.Contains((int)id) ?? false,
            };
            if (viewModel.Product == null)
                return NotFound();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult AddToCart(int? id)
        {
            if (id == null)
                return NotFound();  
            ShoppingCart shoppingCart = GetShoppingCart();
            shoppingCart.ProductsId.Add((int)id);
            HttpContext.Session.Set(WebConstants.ShoppingCartSessionKey, shoppingCart);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int? id)
        {
            if (id == null)
                return NotFound();
            ShoppingCart shoppingCart = GetShoppingCart();
            shoppingCart.ProductsId.Remove((int)id);
            HttpContext.Session.Set(WebConstants.ShoppingCartSessionKey, shoppingCart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private ShoppingCart GetShoppingCart()
        {
            ShoppingCart shoppingCart = HttpContext.Session.Get<ShoppingCart>(WebConstants.ShoppingCartSessionKey);
            return shoppingCart ?? new ShoppingCart();
        }
    }
}