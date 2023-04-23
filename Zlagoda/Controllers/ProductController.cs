using Microsoft.AspNetCore.Mvc;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Models;

namespace Zlagoda.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        [Route("products")]
        [Route("products/list")]
        [JwtAuthorize]
        public async Task<IActionResult> Index([FromQuery(Name = "category")] int? categoryId = null, [FromQuery(Name = "search")] string? productName = null)
        {
            IEnumerable<Product> products = new List<Product>();
            if (categoryId is not null)
            {
                products = await _productRepository.GetProductsByCategoryOrderedByNameAsync(new Category() { category_number = (int)categoryId });
            }
            if (!string.IsNullOrEmpty(productName))
            {
                products = await _productRepository.GetProductsByNameAsync(productName);
            }
            if (categoryId is null && string.IsNullOrEmpty(productName))
            {
                products = await _productRepository.GetAllProductsOrderedByNameAsync();
            }
            var categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync();
            var model = new ProductListViewModel
            {
                Title = "Products",
                Products = products,
                Categories = categories,
                Errors = TempData["Errors"] ?? new List<string>(),
            };
            return View("List", model);
        }

        [HttpGet]
        [Route("products/create")]
        [JwtAuthorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync();
            var model = new CreateProductViewModel
            {
                Title = "Create product",
                Categories = categories,
            };
            return View("Create", model);
        }

        [HttpPost]
        [Route("products/create")]
        [JwtAuthorize]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync();
                    return View("Create", model);
                }
                await _productRepository.CreateProductAsync(model.Product);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                model.Categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync();
                return View("Create", model);
            }
        }

        [HttpGet]
        [Route("products/edit/{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                var categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync();
                var model = new EditProductViewModel
                {
                    Title = "Edit product",
                    Product = product,
                    Categories = categories,
                };
                return View("Edit", model);
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message
                };
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("products/edit/{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync();
                    return View("Edit", model);
                }
                await _productRepository.UpdateProductAsync(model.Product);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                model.Categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync();
                return View("Edit", model);
            }
        }
    }
}
