using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockyShop.DataAccess.Data;
using RockyShop.Model.Models;
using RockyShop.Model.ViewModels;
using System.Diagnostics;
using RockyShop.Utility.Services;

namespace RockyShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly ShoppingCartService _shoppingCartService;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext, ShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _shoppingCartService = shoppingCartService;
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
            var viewModel = new HomeDetailsVM()
            {
                Product = _dbContext.Products
                    .Include(p => p.Category)
                    .Include(p => p.ApplicationType)
                    .Where(p => p.Id == id)
                    .FirstOrDefault(),
                ExistsInCart = _shoppingCartService.CartContains((int)id)
            };
            if (viewModel.Product == null)
                return NotFound();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddToCart(int? id)
        {
            if (id == null)
                return NotFound();  
            _shoppingCartService.AddToCart((int)id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int? id)
        {
            if (id == null)
                return NotFound();
            _shoppingCartService.RemoveFromCart((int)id);
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
    }
}