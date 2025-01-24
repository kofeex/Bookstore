using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork uow, IWebHostEnvironment webHostEnvironment)
        {
            _uow = uow;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _uow.ProductRepository.GetAll(includeProperties: "Category");

            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            var categories = _uow.CategoryRepository.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            var productVM = new ProductVM()
            {
                Categories = categories,
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _uow.ProductRepository.Get(p => p.Id == id);
                return View(productVM);
            }

            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create);
                    file.CopyTo(fileStream);

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _uow.ProductRepository.Add(productVM.Product);
                }
                else
                {
                    _uow.ProductRepository.Update(productVM.Product);
                }

                _uow.Save();
                TempData["success"] = "Product created successfuly";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.Categories = _uow.CategoryRepository.GetAll()
                    .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

                return View(productVM);
            }
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _uow.ProductRepository.GetAll(includeProperties: "Category");

            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _uow.ProductRepository.Get(p =>p.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleteing!" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _uow.ProductRepository.Remove(product);
            _uow.Save();

            return Json(new { success = true, message = "Delete successfull!" });
        }

        #endregion
    }
}
