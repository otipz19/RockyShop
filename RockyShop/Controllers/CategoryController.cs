using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using RockyShop.Utility.Utilities;

namespace RockyShop.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class CategoryController : Controller
    {
		private const string notFoundCategoryMessage = "No category with such id!";
		private readonly ICategoryRepository _categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepo.GetAll();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _categoryRepo.Add(category);
            _categoryRepo.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update([FromRoute]int? id)
        {
            if (id == null)
                return NotFound(notFoundCategoryMessage);

            var toUpdate = _categoryRepo.Find((int)id);
            if (toUpdate == null)
                return NotFound(notFoundCategoryMessage);

            return View(toUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category fromRequest)
        {
            if (!ModelState.IsValid)
                return View(fromRequest);

            var toUpdate = _categoryRepo
                .FirstOrDefault(filter: c => c.Id == fromRequest.Id, isTracking: false);
            if (toUpdate == null)
                return NotFound(notFoundCategoryMessage);

            _categoryRepo.Update(fromRequest);
            _categoryRepo.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int? id)
        {
            if (id == null)
                return NotFound(notFoundCategoryMessage);

            var toDelete = _categoryRepo.Find((int)id);
            if (toDelete == null)
                return NotFound(notFoundCategoryMessage);

            return View(toDelete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category fromRequest)
        {
            var toDelete = _categoryRepo.Find(fromRequest.Id);
            if (toDelete == null)
                return NotFound(notFoundCategoryMessage);

            _categoryRepo.Remove(toDelete);
            _categoryRepo.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
