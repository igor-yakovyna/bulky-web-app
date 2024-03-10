using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var productList = (await _unitOfWork.ProductRepository.GetAll(includeProperties: "Category")).ToList();

            return View(productList);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            var categoryList = (await _unitOfWork.CategoryRepository.GetAll())
                .Select(s => new SelectListItem(s.Name, s.Id.ToString()));

            var productViewModel = new ProductViewModel
            {
                CategoryList = categoryList,
                Product = new Product()
            };

            if (id is null || id == 0)
            {
                return View(productViewModel);
            }
            else
            {
                productViewModel.Product = await _unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == id);
                return View(productViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductViewModel productViewModel, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file is not null)
                {
                    var fileName = string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(file.FileName));
                    var productImagePath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productViewModel.Product?.ImageUrl))
                    {
                        var oldProductImagePath = Path.Combine(wwwRootPath, productViewModel.Product?.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldProductImagePath))
                        {
                            System.IO.File.Delete(oldProductImagePath);
                        }
                    }

                    await using var fileStream = new FileStream(Path.Combine(productImagePath, fileName), FileMode.Create);
                    await file.CopyToAsync(fileStream);

                    if (productViewModel.Product is not null)
                    {
                        productViewModel.Product.ImageUrl = Path.Combine(@"\images\product", fileName);
                    }
                }

                if (productViewModel.Product?.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productViewModel.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productViewModel.Product);
                }

                await _unitOfWork.SaveAsync();

                TempData["SuccessMessage"] = "New product has been successfully created";

                return RedirectToAction("Index");
            }
            else
            {
                var categoryList = (await _unitOfWork.CategoryRepository.GetAll())
                    .Select(s => new SelectListItem(s.Name, s.Id.ToString()));

                productViewModel.CategoryList = categoryList;

                return View(productViewModel);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var product = await _unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product? product)
        {
            if (product is null || product.Id == 0)
            {
                return BadRequest();
            }

            _unitOfWork.ProductRepository.Delete(product);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Product has been successfully deleted";

            return RedirectToAction("Index");
        }
    }
}
