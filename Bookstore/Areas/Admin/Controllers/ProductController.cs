using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _uow;

        public ProductController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IActionResult Index()
        {
            var products = _uow.ProductRepository.GetAll();

            return View(products);
        }

        public IActionResult Create()
        {
            var categories = _uow.CategoryRepository.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _uow.ProductRepository.Add(product);
                _uow.Save();
                TempData["success"] = "Product created successfuly";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var product = _uow.ProductRepository.Get(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _uow.ProductRepository.Update(product);
                _uow.Save();
                TempData["success"] = "Product updated successfuly";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var product = _uow.ProductRepository.Get(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var prodcut = _uow.ProductRepository.Get(p => p.Id == id);

            if (prodcut == null)
            {
                return NotFound();
            }

            _uow.ProductRepository.Remove(prodcut);
            _uow.Save();
            TempData["success"] = "Product deleted successfuly";

            return RedirectToAction("Index");
        }
    }
}
