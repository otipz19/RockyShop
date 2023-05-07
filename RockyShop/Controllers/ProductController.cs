using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockyShop.Data;
using RockyShop.Models;
using RockyShop.Models.ViewModels;
using RockyShop.Services;

namespace RockyShop.Controllers
{
	public class ProductController : Controller
	{
		private readonly AppDbContext _dbContext;
        private readonly ProductImageService _productImageService;

        public ProductController(AppDbContext dbContext, ProductImageService productImageService)
        {
            _dbContext = dbContext;
            _productImageService = productImageService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = _dbContext.Products.ToList();
            foreach(var product in products)
            {
                product.Category = _dbContext.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }
            return View(products);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            var viewModel = new ProductVM()
            {
                CategoryDropDown = GetCategoryDropDown(),
                Product = new Product()
            };

            if (id == null)
                return View(viewModel);

            viewModel.Product = _dbContext.Products.Find(id);
            if (viewModel.Product == null)
                return NotFound();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM fromRequest)
        {
            fromRequest.CategoryDropDown = GetCategoryDropDown();
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
                _dbContext.Products.Add(fromRequest.Product);
            }
            //Update
            else
            {
                string oldFileName = _dbContext.Products
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == fromRequest.Product.Id)
                    .Image;
                fromRequest.Product.Image = oldFileName;
                if (uploadedFiles.Count != 0)
                {
                    _productImageService.DeleteImage(oldFileName);
                    fromRequest.Product.Image = newFileName;
                }

                _dbContext.Products.Update(fromRequest.Product);
            }

            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var toDelete = _dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .FirstOrDefault();
            if (toDelete == null)
                return NotFound();
            //toDelete.Category = _dbContext.Categories.Find(toDelete.CategoryId);
            return View(toDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var toDelete = _dbContext.Products.Find(id);
            if (toDelete == null)
                return NotFound();
            _productImageService.DeleteImage(toDelete.Image);
            _dbContext.Products.Remove(toDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        private IEnumerable<SelectListItem> GetCategoryDropDown()
        {
            return _dbContext.Categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }
    }
}
