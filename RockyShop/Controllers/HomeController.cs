using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockyShop.DataAccess.Data;
using RockyShop.Model.Models;
using RockyShop.Model.ViewModels;
using System.Diagnostics;
using RockyShop.Utility.Services;
using RockyShop.DataAccess.Repository.Interfaces;

namespace RockyShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ShoppingCartService _shoppingCartService;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepo,
            ICategoryRepository categoryRepo, ShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeIndexVM()
            {
                Products = _productRepo.GetAllIncludeAll().OrderBy(p => p.Category.DisplayOrder),
                Categories = _categoryRepo.GetAll(),
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            var viewModel = new ProductInCart()
            {
                Product = _productRepo.FirstOrDefaultIncludeAll(p => p.Id == id),
                ExistsInCart = _shoppingCartService.CartContains((int)id)
            };
            if (viewModel.Product == null)
                return base.NotFound();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(ProductInCart productInCart)
        {
            //if (!ModelState.IsValid)
            //    return RedirectToAction(nameof(Details), routeValues: new { id = homeDetailsVM.Product.Id });
            _shoppingCartService.AddToCart(productInCart.Product.Id, productInCart.SqFt);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
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