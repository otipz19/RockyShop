using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockyShop.Utility.Interfaces;
using RockyShop.Model.ViewModels;
using RockyShop.Utility.Services;
using System.Security.Claims;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;

namespace RockyShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IAppUserRepository _appUserRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;
        private readonly IInquiryDetailsRepository _inquiryDetailsRepo;
        private readonly ShoppingCartService _shoppingCartService;

        public CartController(IAppUserRepository appUserRepo,
            IProductRepository productRepo,
            IInquiryHeaderRepository inquiryHeaderRepo,
            IInquiryDetailsRepository inquiryDetailsRepo,
            ShoppingCartService shoppingCartService)
        {
            _appUserRepo = appUserRepo;
            _productRepo = productRepo;
            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailsRepo = inquiryDetailsRepo;
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
                User = _appUserRepo.Find(username.Value)
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
            CartUserVM.Products = _productRepo.GetAll(filter: p => productIds.Contains(p.Id)).ToList();

            var inquiryHeader = new InquiryHeader()
            {
                AppUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                InquiryTime = DateTime.Now,
                Email = CartUserVM.User.Email,
                PhoneNumber = CartUserVM.User.PhoneNumber,
                FullName = CartUserVM.User.FullName,
            };
            _inquiryHeaderRepo.Add(inquiryHeader);
            _inquiryHeaderRepo.SaveChanges();

            foreach(var product in CartUserVM.Products)
            {
                var inquiryDetails = new InquiryDetails()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = product.Id,
                };
                _inquiryDetailsRepo.Add(inquiryDetails);
            }
            _inquiryDetailsRepo.SaveChanges();

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
