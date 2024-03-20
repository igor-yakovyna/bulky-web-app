using System.Diagnostics;
using System.Security.Claims;
using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _unitOfWork.ProductRepository.GetAll(includeProperties: "Category");

        return View(products);
    }

    public async Task<IActionResult> Details(int productId)
    {
        var shoppingCart = new ShoppingCart
        {
            Product = await _unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == productId, includeProperties: "Category") ?? new Product(),
            ProductId = productId,
            Count = 1
        };

        return View(shoppingCart);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Details(ShoppingCart shoppingCart)
    {
        var userClaimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = userClaimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        shoppingCart.ApplicationUserId = userId;

        var existingShoppingCart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(p => p.ApplicationUserId == userId && p.ProductId == shoppingCart.ProductId);
        if (existingShoppingCart is not null)
        {
            existingShoppingCart.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCartRepository.Update(existingShoppingCart);
        }
        else
        {
            _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
        }

        await _unitOfWork.ShoppingCartRepository.SaveAsync();

        return RedirectToAction(nameof(Index));
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
