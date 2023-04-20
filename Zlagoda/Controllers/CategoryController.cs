using Microsoft.AspNetCore.Mvc;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Enums;

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
        [Route("category/delete/{id}")]
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
    }
}
