using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockyShop.Data;
using RockyShop.Interfaces;
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

        [HttpGet]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost([FromServices]IEmailSenderService emailSenderService)
        {
            _shoppingCartService.ClearCart();

            //Restore products from db by id
            IEnumerable<int> productIds = CartUserVM.Products
                .Select(p => p.Id);
            CartUserVM.Products = await _dbContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            await emailSenderService.SendInquiryConfirmationEmailAsync(CartUserVM);

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        [HttpGet]
        public IActionResult InquiryConfirmation()
        {
            return View();
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
