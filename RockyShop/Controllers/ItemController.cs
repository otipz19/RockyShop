using Microsoft.AspNetCore.Mvc;
using RockyShop.Data;
using RockyShop.Models;

namespace RockyShop.Controllers
{
	public class ItemController : Controller
	{
		private readonly AppDbContext _dbContext;

        public ItemController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var items = _dbContext.Items.AsEnumerable();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Item item)
        {
            if (!ModelState.IsValid)
                return View(item);

            _dbContext.Items.Add(item);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
