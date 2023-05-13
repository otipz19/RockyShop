using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockyShop.DataAccess.Data;
using RockyShop.Model.Models;
using RockyShop.Utility.Utilities;
using System.Data;

namespace RockyShop.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class CategoryController : Controller
    {
		private const string notFoundCategoryMessage = "No category with such id!";
		private readonly AppDbContext _dbContext;

        public CategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var categories = _dbContext.Categories.AsEnumerable();
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

            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update([FromRoute]int? id)
        {
            //if (id == null || id <= 0)
            //    return NotFound(notFoundCategoryMessage);

            var toUpdate = _dbContext.Categories.Find(id);
            if (toUpdate == null)
                return NotFound(notFoundCategoryMessage);

            return View(toUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category fromRequest)
        {
            var toUpdate = _dbContext.Categories
                .AsNoTracking()
                .Where(c => c.Id == fromRequest.Id)
                .FirstOrDefault();
            if (toUpdate == null)
                return NotFound(notFoundCategoryMessage);

            if (!ModelState.IsValid)
                return View(fromRequest);

            _dbContext.Categories.Update(fromRequest);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete([FromRoute]int? id)
        {
			//if (id == null || id <= 0)
			//	return NotFound(notFoundCategoryMessage);

			var toDelete = _dbContext.Categories.Find(id);
            if (toDelete == null)
                return NotFound(notFoundCategoryMessage);

            return View(toDelete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category fromRequest)
        {
            var toDelete = _dbContext.Categories.Find(fromRequest.Id);
            if (toDelete == null)
                return NotFound(notFoundCategoryMessage);

            _dbContext.Categories.Remove(toDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
