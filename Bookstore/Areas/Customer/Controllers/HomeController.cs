using System.Diagnostics;
using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _uow;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _uow.ProductRepository.GetAll(includeProperties: "Category");

            return View(products);
        }

        public IActionResult Details(int id)
        {
            Product product = _uow.ProductRepository.Get(p => p.Id == id, includeProperties: "Category");

            return View(product);
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
