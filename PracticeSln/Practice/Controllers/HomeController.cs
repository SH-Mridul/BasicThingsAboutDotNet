using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Practice.Models;
using Practice.Models.Item;
using Practice.Models.product;

namespace Practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IItem _item;
        private readonly IProduct _electronics;
        private readonly IProduct _vhicle;

        public HomeController(ILogger<HomeController> logger,IItem item, [FromKeyedServices("electronics")] IProduct electronics, [FromKeyedServices("vhicle")] IProduct vhicle)
        {
            _logger = logger;
            _item = item;
            _electronics = electronics;
            _vhicle = vhicle;
        }

        public IActionResult Index()
        {

            var amount = _item.GetAmount();
            var vPrice = _vhicle.GetProductPrice();
            var ePrice = _electronics.GetProductPrice();
            _logger.LogInformation("I am in index page.");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
