using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using RockyShop.Model.ViewModels;
using RockyShop.Utility.Services;
using RockyShop.Utility.Utilities;

namespace RockyShop.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
	public class ProductController : Controller
	{
		private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IApplicationTypeRepository _appTypeRepo;
        private readonly ProductImageService _productImageService;

        public ProductController(IProductRepository productRepo,
            ICategoryRepository categoryRepo,
            IApplicationTypeRepository appTypeRepo,
            ProductImageService productImageService)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _appTypeRepo = appTypeRepo;
            _productImageService = productImageService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = _productRepo.GetAllIncludeAll();
            return View(products);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            var viewModel = new ProductVM();
            PopulateProductVM(viewModel);

            if (id == null)
                return View(viewModel);

            viewModel.Product = _productRepo.FirstOrDefaultIncludeAll(p => p.Id == id);
            if (viewModel.Product == null)
                return NotFound();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM fromRequest)
        {
            PopulateProductVM(fromRequest);
            TempData[Constants.NotificationError] = "Error on action!";
            if (!ModelState.IsValid)
                return View(fromRequest);

            IFormFileCollection uploadedFiles = HttpContext.Request.Form.Files;
            string newFileName = null;
            if (uploadedFiles.Count != 0)
            {
                try
                {
                    newFileName = _productImageService.UploadImage(uploadedFiles[0]);
                }
                catch (ApplicationException e)
                {
                    //Doesn't work for some reason
                    //ModelState.AddModelError("Image", e.Message);
                    return View(fromRequest);
                }
            }

            //Create
            if (fromRequest.Product.Id == 0)
            {
                //No file was uploaded
                if (uploadedFiles.Count == 0)
                    return View(fromRequest);

                fromRequest.Product.Image = newFileName;
                _productRepo.Add(fromRequest.Product);
            }
            //Update
            else
            {
                var toUpdate = _productRepo
                    .FirstOrDefault(filter: p => p.Id == fromRequest.Product.Id, isTracking: false);
                if (toUpdate == null)
                    return NotFound();
                string oldFileName = toUpdate.Image;
                fromRequest.Product.Image = oldFileName;
                if (uploadedFiles.Count != 0)
                {
                    _productImageService.DeleteImage(oldFileName);
                    fromRequest.Product.Image = newFileName;
                }

                _productRepo.Update(fromRequest.Product);
            }

            _productRepo.SaveChanges();
			TempData[Constants.NotificationSuccess] = "Action was made successfully!";
			return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var toDelete = _productRepo.FirstOrDefaultIncludeAll(p => p.Id == id);
            if (toDelete == null)
                return NotFound();
            return View(toDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null)
                return NotFound();
            var toDelete = _productRepo.Find((int)id);
            if (toDelete == null)
                return NotFound();
            _productImageService.DeleteImage(toDelete.Image);
            _productRepo.Remove(toDelete);
            _productRepo.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateProductVM(ProductVM viewModel)
        {
            viewModel.CategoryDropDown = _categoryRepo.GetAll().Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            viewModel.AppTypeDropDown = _appTypeRepo.GetAll().Select(a => new SelectListItem()
            {
                Text = a.Name,
                Value = a.Id.ToString()
            });
        }
    }
}
