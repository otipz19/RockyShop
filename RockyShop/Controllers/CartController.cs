using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockyShop.Data;
using RockyShop.Models.ViewModels;
using RockyShop.Services;
using System.Security.Claims;

namespace RockyShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ShoppingCartService _shoppingCartService;

        public CartController(AppDbContext dbContext, ShoppingCartService shoppingCartService)
        {
            _dbContext = dbContext;
            _shoppingCartService = shoppingCartService;
        }

        [BindProperty]
        public CartUserVM CartUserVM { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_shoppingCartService.GetProductsFromCart());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier);
            CartUserVM = new CartUserVM()
            {
                Products = _shoppingCartService.GetProductsFromCart(),
                User = _dbContext.AppUsers.Find(username.Value)
            };
            return View(model:CartUserVM);
        }

        [HttpGet]
        public IActionResult Remove(int? id)
        {
            if (id == null)
                return NotFound();
            _shoppingCartService.RemoveFromCart((int)id);
            return RedirectToAction(nameof(Index));
        }
    }
}
