using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Models;
using RockyShop.Utility.Utilities;
using System.Data;

namespace RockyShop.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _appTypeRepo;

        public ApplicationTypeController(IApplicationTypeRepository appTypeRepo)
        {
            _appTypeRepo = appTypeRepo;    
        }

        public IActionResult Index()
        {
            var appTypes = _appTypeRepo.GetAll();
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
            _appTypeRepo.Add(fromRequest);
            _appTypeRepo.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null)
                return NotFound();
            var toUpdate = _appTypeRepo.Find((int)id);
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

            var toUpdate = _appTypeRepo
                .FirstOrDefault(filter: a => a.Id == fromRequest.Id, isTracking: false);
            if (toUpdate == null)
                return NotFound();

            _appTypeRepo.Update(fromRequest);
            _appTypeRepo.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var toDelete = _appTypeRepo.Find((int)id);
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
            var toDelete = _appTypeRepo.Find((int)id);
            if (toDelete == null)
                return NotFound();
            _appTypeRepo.Remove(toDelete);
            _appTypeRepo.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
