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
using RockyShop.Model.Enums;
using Braintree;

namespace RockyShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IAppUserRepository _appUserRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;
        private readonly IInquiryDetailsRepository _inquiryDetailsRepo;
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailsRepository _orderDetailsRepo;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly IBraintreeService _braintreeService;
        private readonly IEmailSenderService _emailSenderService;

        public CartController(IAppUserRepository appUserRepo,
            IProductRepository productRepo,
            IInquiryHeaderRepository inquiryHeaderRepo,
            IInquiryDetailsRepository inquiryDetailsRepo,
            IOrderHeaderRepository orderHeaderRepo,
            IOrderDetailsRepository orderDetailsRepo,
            ShoppingCartService shoppingCartService,
            IBraintreeService braintreeService,
            IEmailSenderService emailSenderService)
        {
            _appUserRepo = appUserRepo;
            _productRepo = productRepo;
            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailsRepo = inquiryDetailsRepo;
            _shoppingCartService = shoppingCartService;
            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailsRepo = orderDetailsRepo;
            _braintreeService = braintreeService;
            _emailSenderService = emailSenderService;
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
            if(ModelState.IsValid)
                UpdateCartState(productInCartList);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Continue(IEnumerable<ProductInCart> productInCartList)
        {
            if (ModelState.IsValid)
            {
                UpdateCartState(productInCartList);
                return RedirectToAction(nameof(Summary));
            }
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Summary()
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

                ViewBag.ClientToken = await _braintreeService.GetClientTokenAsync();
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
        public async Task<IActionResult> SummaryPost(IFormCollection formCollection)
        {
            if (User.IsInRole(Constants.AdminRole))
            {
                return await SubmitOrder(formCollection);
            }
            else
            {
                return await SubmitInquiry();
            }
        }

        [HttpGet]
        public IActionResult InquiryConfirmation(int id)
        {
            if (id != 0)
                return View(_orderHeaderRepo.Find(id));
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

        [HttpGet]
        public IActionResult ClearCart()
        {
            _shoppingCartService.ResetCart();
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> SubmitInquiry()
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

            foreach (var productInCart in CartUserVM.ProductInCartList)
            {
                var inquiryDetails = new InquiryDetails()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = productInCart.Product.Id,
                };
                _inquiryDetailsRepo.Add(inquiryDetails);
            }
            _inquiryDetailsRepo.SaveChanges();

            await _emailSenderService.SendInquiryConfirmationEmailAsync(CartUserVM);

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        private async Task<IActionResult> SubmitOrder(IFormCollection formCollection)
        {
            _shoppingCartService.ResetCart();

            IEnumerable<int> productIds = CartUserVM.ProductInCartList.Select(p => p.Product.Id);
            IEnumerable<Product> products = _productRepo.GetAll(p => productIds.Contains(p.Id));
            foreach (var productInCart in CartUserVM.ProductInCartList)
            {
                productInCart.Product = products.First(p => p.Id == productInCart.Product.Id);
            }
            
            var orderHeader = new OrderHeader()
            {
                CreatedByUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.Pending,
                FinalOrderTotal = CartUserVM.ProductInCartList.Sum(p => p.SqFt * p.Product.Price),
                FullName = CartUserVM.User.FullName,
                PhoneNumber = CartUserVM.User.PhoneNumber,
                Email = CartUserVM.User.Email,
                City = CartUserVM.User.City,
                State = CartUserVM.User.State,
                Street = CartUserVM.User.Street,
                PoastalCode = CartUserVM.User.PoastalCode,
            };
            _orderHeaderRepo.Add(orderHeader);
            _orderHeaderRepo.SaveChanges();

            foreach (var productInCart in CartUserVM.ProductInCartList)
            {
                var orderDetails = new OrderDetails()
                {
                    OrderHeaderId = orderHeader.Id,
                    ProductId = productInCart.Product.Id,
                    SqFt = productInCart.SqFt,
                    PricePerSqFt = productInCart.Product.Price,
                };
                _orderDetailsRepo.Add(orderDetails);
            };
            _orderDetailsRepo.SaveChanges();

            string nonce = formCollection["payment_method_nonce"];
            await _braintreeService.ProcessTransactionAsync(orderHeader, nonce);
            _orderHeaderRepo.SaveChanges();

            return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });
        }

        private void UpdateCartState(IEnumerable<ProductInCart> productInCartList)
        {
            _shoppingCartService.ClearCartItems();
            _shoppingCartService.AddToCartRange(productInCartList);
        }
    }
}
