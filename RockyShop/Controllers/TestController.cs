using Microsoft.AspNetCore.Mvc;

namespace RockyShop.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index(int? id, string keyword)
        {
            return Ok($"Passed id: {id}\nPassed keyword: {keyword}");
        }
    }
}
