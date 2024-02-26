using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categoryList = (await _unitOfWork.CategoryRepository.GetAll()).ToList();

            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order cannot exactly match the Name");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Add(category);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMessage"] = "New category has been successfully created";

                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefault(p => p.Id == id);
            if (category is null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMessage"] = "Category has been successfully updated";

                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefault(p => p.Id == id);
            if (category is null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Category? category)
        {
            if (category is null || category.Id == 0)
            {
                return BadRequest();
            }

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Category has been successfully deleted";

            return RedirectToAction("Index");
        }
    }
}
