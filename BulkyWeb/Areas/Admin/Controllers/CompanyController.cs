using BulkyBook.DataAccess.Repository.Interfaces;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticDetails.AdminRole)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var companyList = (await _unitOfWork.CompanyRepository.GetAll()).ToList();

        return View(companyList);
    }

    public async Task<IActionResult> Upsert(int? id)
    {
        if (id is null || id == 0)
        {
            return View(new Company());
        }
        else
        {
            var company = await _unitOfWork.CompanyRepository.GetFirstOrDefault(p => p.Id == id);
            return View(company);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(Company company)
    {
        if (ModelState.IsValid)
        {
            if (company.Id == 0)
            {
                _unitOfWork.CompanyRepository.Add(company);
            }
            else
            {
                _unitOfWork.CompanyRepository.Update(company);
            }

            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "New company has been successfully created";

            return RedirectToAction("Index");
        }
        else
        {
            return View(company);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        var company = await _unitOfWork.CompanyRepository.GetFirstOrDefault(p => p.Id == id);
        if (company is null)
        {
            return NotFound();
        }

        return View(company);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Company? company)
    {
        if (company is null || company.Id == 0)
        {
            return BadRequest();
        }

        _unitOfWork.CompanyRepository.Delete(company);
        await _unitOfWork.SaveAsync();

        TempData["SuccessMessage"] = "company has been successfully deleted";

        return RedirectToAction("Index");
    }
}
