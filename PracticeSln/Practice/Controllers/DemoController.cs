using Microsoft.AspNetCore.Mvc;
using Practice.Models;

namespace Practice.Controllers
{
    public class DemoController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new DemoModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(DemoModel model) 
        { 
            return View(model);
        }

    }
}
