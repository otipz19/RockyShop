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
    public class ApplicationTypeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ApplicationTypeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;    
        }

        public IActionResult Index()
        {
            var appTypes = _dbContext.ApplicationTypes;
            return View(appTypes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType fromRequest)
        {
            if (!ModelState.IsValid)
                return View(fromRequest);
            _dbContext.ApplicationTypes.Add(fromRequest);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            var toUpdate = _dbContext.ApplicationTypes.Find(id);
            if (toUpdate == null)
                return NotFound();
            return View(toUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ApplicationType fromRequest)
        {
            if (!ModelState.IsValid)
                return View(fromRequest);

            var toUpdate = _dbContext.ApplicationTypes
                    .AsNoTracking()
                    .Where(a => a.Id == fromRequest.Id)
                    .FirstOrDefault();
            if (toUpdate == null)
                return NotFound();

            _dbContext.ApplicationTypes.Update(fromRequest);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var toDelete = _dbContext.ApplicationTypes.Find(id);
            if (toDelete == null)
                return NotFound();
            return View(toDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var toDelete = _dbContext.ApplicationTypes.Find(id);
            if (toDelete == null)
                return NotFound();
            _dbContext.ApplicationTypes.Remove(toDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
