using System.Security.Claims;
using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CartController(ILogger<CartController> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var userClaimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = userClaimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var shoppingCartViewModel = new ShoppingCartViewModel
        {
            ShoppingCarts = await _unitOfWork.ShoppingCartRepository.GetAll(p => p.ApplicationUserId == userId, includeProperties: "Product")
        };

        foreach (var shoppingCart in shoppingCartViewModel.ShoppingCarts)
        {
            shoppingCart.Price = GetPriceBasedOnQuantity(shoppingCart);
            shoppingCartViewModel.OrderTotal += shoppingCart.Price * shoppingCart.Count;
        }

        return View(shoppingCartViewModel);
    }

    public async Task<IActionResult> Summary()
    {
        return View();
    }

    public async Task<IActionResult> Plus(int cardId)
    {
        var shoppingCard = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(p => p.Id == cardId);

        if (shoppingCard is not null)
        {
            shoppingCard.Count += 1;
            _unitOfWork.ShoppingCartRepository.Update(shoppingCard);
            await _unitOfWork.SaveAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Minus(int cardId)
    {
        var shoppingCard = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(p => p.Id == cardId);

        if (shoppingCard is not null)
        {
            if (shoppingCard.Count <= 1)
            {
                _unitOfWork.ShoppingCartRepository.Delete(shoppingCard);
            }
            else
            {
                shoppingCard.Count -= 1;
                _unitOfWork.ShoppingCartRepository.Update(shoppingCard);
            }

            await _unitOfWork.SaveAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Remove(int cardId)
    {
        var shoppingCard = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(p => p.Id == cardId);

        if (shoppingCard is not null)
        {
            _unitOfWork.ShoppingCartRepository.Delete(shoppingCard);
            await _unitOfWork.SaveAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private static double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }

        if (shoppingCart.Count <= 100)
        {
            return shoppingCart.Product.PriceFifty;
        }

        return shoppingCart.Product.PriceHundred;

    }
}
