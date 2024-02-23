using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var categoryList = _dbContext.Categories.ToList();

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
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "New category has been successfully created";

                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var category = _dbContext.Categories.Find(id);
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
                _dbContext.Categories.Update(category);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category has been successfully updated";

                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var category = _dbContext.Categories.Find(id);
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

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category has been successfully deleted";

            return RedirectToAction("Index");
        }
    }
}
