using Microsoft.AspNetCore.Mvc;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Business.Repositories;
using Zlagoda.Enums;
using Zlagoda.Models;

namespace Zlagoda.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository) 
        { 
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [Route("categories")]
        [Route("categories/list")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync();
            var model = new
            {
                Title = "Categories",
                Categories = categories,
                Errors = TempData["Errors"] ?? new List<string>(),
            };
            return View("List", model);
        }

        [HttpGet]
        [Route("categories/delete/{id}")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryRepository.DeleteCategoryAsync(new Category() { category_number = id });
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message,
                };
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("categories/create")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public IActionResult Create()
        {
            var model = new CreateCategoryViewModel
            {
                Title = "Create client",
                Category = new Category(),
            };
            return View("Create", model);
        }

        [HttpPost]
        [Route("categories/create")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", model);
                }
                await _categoryRepository.CreateCategoryAsync(model.Category);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                return View("Create", model);
            }
        }

        [HttpGet]
        [Route("categories/edit/{id}")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByNumberAsync(id);
                var model = new EditCategoryViewModel
                {
                    Title = "Edit category",
                    Category = category,
                };
                return View("Edit", model);
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>()
                {
                    exception.Message,
                };
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("categories/edit/{id}")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }
                await _categoryRepository.UpdateCategoryAsync(model.Category);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                return View("Edit", model);
            }
        }
    }
}
