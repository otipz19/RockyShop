using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockyShop.Utility.Interfaces;
using RockyShop.Model.ViewModels;
using RockyShop.Utility.Services;
using System.Security.Claims;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using RockyShop.Utility.Utilities;
using NuGet.Versioning;

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
            return View(_shoppingCartService.GetProductsInCart());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<ProductInCart> productInCartList)
        {
            UpdateCartState(productInCartList);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Continue(IEnumerable<ProductInCart> productInCartList)
        {
            UpdateCartState(productInCartList);
            return RedirectToAction(nameof(Summary));
        }

        [HttpGet]
        public IActionResult Summary()
        {
            AppUser appUser;
            if (User.IsInRole(Constants.AdminRole))
            {
                if (_shoppingCartService.IsCartFromInquiry)
                {
                    InquiryHeader inquiryHeader = _inquiryHeaderRepo.Find(_shoppingCartService.InquiryId);
                    if (inquiryHeader == null)
                        return NotFound();
                    appUser = new AppUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber,
                    };
                }
                else
                {
                    appUser = new AppUser();
                }
            }
            else
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier);
                appUser = _appUserRepo.Find(username.Value);
            }

            CartUserVM = new CartUserVM()
            {
                ProductInCartList = _shoppingCartService.GetProductsInCart(),
                User = appUser,
            };
            return View(model:CartUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost([FromServices]IEmailSenderService emailSenderService)
        {
            _shoppingCartService.ClearCartItems();

            //Restore products from db by id
            IEnumerable<int> productIds = CartUserVM.ProductInCartList
                .Select(p => p.Product.Id);
            CartUserVM.ProductInCartList = _productRepo
                .GetAll(filter: p => productIds.Contains(p.Id))
                .Select(p => new ProductInCart() { Product = p })
                .ToList();

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

            foreach(var productInCart in CartUserVM.ProductInCartList)
            {
                var inquiryDetails = new InquiryDetails()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = productInCart.Product.Id,
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

        private void UpdateCartState(IEnumerable<ProductInCart> productInCartList)
        {
            _shoppingCartService.ClearCartItems();
            _shoppingCartService.AddToCartRange(productInCartList);
        }
    }
}
