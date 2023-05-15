using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using RockyShop.Model.ViewModels;
using RockyShop.Utility.Services;
using RockyShop.Utility.Utilities;

namespace RockyShop.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;
        private readonly IInquiryDetailsRepository _inquiryDetailsRepo;
        private readonly ShoppingCartService _shoppingCartService;

        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepo,
            IInquiryDetailsRepository inquiryDetailsRepo,
            ShoppingCartService shoppingCartService)
        {
            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailsRepo = inquiryDetailsRepo;
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            InquiryVM inquiryVM;
            try
            {
                inquiryVM = GetInquiryVM(id);
            }
            catch
            {
                return NotFound();
            }
            return View(inquiryVM);
        }

        [HttpPost]
        [ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult ConvertToCart(int id)
        {
            InquiryVM inquiryVM;
            try
            {
                inquiryVM = GetInquiryVM(id);
            }
            catch
            {
                return NotFound();
            }
            var products = inquiryVM.InquiryDetailsList
                .Select(d => d.Product);
            _shoppingCartService.ClearCart();
            _shoppingCartService.AddToCartRange(products);
            return RedirectToAction(controllerName: "Cart", actionName: "Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            InquiryVM inquiryVM;
            try
            {
                inquiryVM = GetInquiryVM(id);
            }
            catch
            {
                return NotFound();
            }
            _inquiryHeaderRepo.Remove(inquiryVM.InquiryHeader);
            _inquiryDetailsRepo.RemoveRange(inquiryVM.InquiryDetailsList);
            _inquiryDetailsRepo.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private InquiryVM GetInquiryVM(int id)
        {
            InquiryHeader inquiryHeader = _inquiryHeaderRepo.Find(id);
            if (inquiryHeader == null)
                throw new ApplicationException();
            IEnumerable<InquiryDetails> inquiryDetailsList = _inquiryDetailsRepo
                .GetAllIncludeProduct(d => d.InquiryHeaderId == inquiryHeader.Id, isTracking: false);
            return new InquiryVM()
            {
                InquiryHeader = inquiryHeader,
                InquiryDetailsList = inquiryDetailsList,
            };
        }

        #region API CALLS

        public IActionResult GetAllInquires()
        {
            return Json(new { data = _inquiryHeaderRepo.GetAll(isTracking: false) });
        }

        #endregion
    }
}
