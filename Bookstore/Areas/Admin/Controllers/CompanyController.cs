using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _uow;

        public CompanyController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IActionResult Index()
        {
            var companies = _uow.CompanyRepository.GetAll();

            return View(companies);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                var companies = _uow.CompanyRepository.Get(c => c.Id == id);
                return View(companies);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _uow.CompanyRepository.Add(company);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _uow.CompanyRepository.Update(company);
                    TempData["success"] = "Company updated successfully";
                }

                _uow.Save();
                
                return RedirectToAction("Index");
            }

            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _uow.CompanyRepository.GetAll();

            return Json(new { data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var company = _uow.CompanyRepository.Get(c => c.Id == id);

            if (company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _uow.CompanyRepository.Remove(company);
            _uow.Save();

            return Json(new { success = true, message = "Company deleted successfully"});
        }

        #endregion
    }
}
