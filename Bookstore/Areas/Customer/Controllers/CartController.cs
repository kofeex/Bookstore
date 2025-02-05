using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookstore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _uow;
        public ShoppingCartVM ShoppingCartVM { get; set; } = new();

        public CartController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _uow.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product")
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            return View();
        }

        public IActionResult Plus(int id)
        {
            var cartFromDb = _uow.ShoppingCartRepository.Get(c => c.Id == id);
            cartFromDb.Count += 1;

            _uow.ShoppingCartRepository.Update(cartFromDb);
            _uow.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int id)
        {
            var cartFromDb = _uow.ShoppingCartRepository.Get(c => c.Id == id);

            if (cartFromDb.Count <= 1)
            {
                _uow.ShoppingCartRepository.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _uow.ShoppingCartRepository.Update(cartFromDb);
            }

            _uow.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            var cartFromDb = _uow.ShoppingCartRepository.Get(c => c.Id == id);

            _uow.ShoppingCartRepository.Remove(cartFromDb);
            _uow.Save();

            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else if (cart.Count <= 100)
            {
                return cart.Product.Price50;
            }
            else
            {
                return cart.Product.Price100;
            }
        }
    }
}
