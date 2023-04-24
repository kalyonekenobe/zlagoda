using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Enums;
using Zlagoda.Models;
using Zlagoda.Services;

namespace Zlagoda.Controllers
{
    public class StoreProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreProductRepository _storeProductRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StoreProductController(IProductRepository productRepository, IStoreProductRepository storeProductRepository, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _storeProductRepository = storeProductRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("store-products")]
        [Route("store-products/list")]
        [JwtAuthorize]
        public async Task<IActionResult> Index([FromQuery(Name = "promo")] string? promo = null, [FromQuery(Name = "sorting")] string? sorting = null)
        {
            IEnumerable<StoreProduct> storeProducts = new List<StoreProduct>(); 
            if (promo is null)
            {
                if (sorting != "quantity")
                {
                    storeProducts = await _storeProductRepository.GetAllStoreProductsOrderedByNameAsync();
                }
                else
                {
                    storeProducts = await _storeProductRepository.GetAllStoreProductsOrderedByQuantityAsync();
                }
            }
            else
            {
                if (promo == "true")
                {
                    if (sorting != "quantity")
                    {
                        storeProducts = await _storeProductRepository.GetAllPromotionalStoreProductsOrderedByNameThenByQuantityAsync();
                    }
                    else
                    {
                        storeProducts = await _storeProductRepository.GetAllPromotionalStoreProductsOrderedByQuantityThenByNameAsync();
                    }
                }
                else
                {
                    if (sorting != "quantity")
                    {
                        storeProducts = await _storeProductRepository.GetAllNonPromotionalStoreProductsOrderedByNameThenByQuantityAsync();
                    }
                    else
                    {
                        storeProducts = await _storeProductRepository.GetAllNonPromotionalStoreProductsOrderedByQuantityThenByNameAsync();
                    }
                }
            }
            
            var model = new StoreProductListViewModel
            {
                Title = "Store products list",
                StoreProducts = storeProducts,
                Promo = promo,
                Sorting = sorting,
                Errors = TempData["Errors"] ?? new List<string>(),
            }; 
            return View("List", model);
        }

        [HttpGet]
        [Route("store-products/delete/{id}")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _storeProductRepository.DeleteStoreProductAsync(new StoreProduct { UPC = id });
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
        [Route("store-products/create")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var products = await _productRepository.GetAllProductsOrderedByNameAsync();
                var nonPromotionalStoreProducts = await _storeProductRepository.GetAllNonPromotionalStoreProductsOrderedByNameThenByQuantityAsync();
                var model = new CreateStoreProductViewModel
                {
                    Title = "Create store product",
                    Products = products,
                    NonPromotionalStoreProducts = nonPromotionalStoreProducts,
                };
                return View("Create", model);
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message,
                };
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("store-products/create")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Create(CreateStoreProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.NonPromotionalStoreProducts = await _storeProductRepository.GetAllNonPromotionalStoreProductsOrderedByNameThenByQuantityAsync();
                    model.Products = await _productRepository.GetAllProductsOrderedByNameAsync();
                    return View("Create", model);
                }
                if (model.StoreProduct.promotional_product)
                {
                    if (model.ParentUPC is null)
                    {
                        throw new Exception("Non promotional store product with such UPC doesn't exist!");
                    }
                    var parentProduct = await _storeProductRepository.GetStoreProductByUPCAsync(model.ParentUPC);
                    model.StoreProduct.selling_price = parentProduct.selling_price * 0.8m;
                    model.StoreProduct.id_product = parentProduct.id_product;
                    parentProduct.UPC_prom = model.StoreProduct.UPC;
                    await _storeProductRepository.CreateStoreProductAsync(model.StoreProduct);
                    await _storeProductRepository.UpdateStoreProductAsync(parentProduct);
                }
                else
                {
                    await _storeProductRepository.CreateStoreProductAsync(model.StoreProduct);
                }
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                model.NonPromotionalStoreProducts = await _storeProductRepository.GetAllNonPromotionalStoreProductsOrderedByNameThenByQuantityAsync();
                model.Products = await _productRepository.GetAllProductsOrderedByNameAsync();
                return View("Create", model);
            }
        }

        [HttpGet]
        [Route("store-products/edit/{id}")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var products = await _productRepository.GetAllProductsOrderedByNameAsync();
                var storeProduct = await _storeProductRepository.GetStoreProductByUPCAsync(id);
                var model = new EditStoreProductViewModel
                {
                    Title = "Edit store product",
                    Products = products,
                    StoreProduct = storeProduct,
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
        [Route("store-products/edit/{id}")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Edit(EditStoreProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Products = await _productRepository.GetAllProductsOrderedByNameAsync();
                    return View("Edit", model);
                }
                await _storeProductRepository.UpdateStoreProductAsync(model.StoreProduct);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                model.Products = await _productRepository.GetAllProductsOrderedByNameAsync();
                return View("Edit", model);
            }
        }

        [HttpGet]
        [Route("store-products/brief-info/{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> BriefInfo(string id)
        {
            try
            {
                var user = ClaimsService.GetUserFromClaims(_httpContextAccessor.HttpContext!.User);
                dynamic model;
                if (user.empl_role == nameof(UserRoles.Manager))
                {
                    var query = await _storeProductRepository.GetStoreProductPriceQuantityNameAndCharacteristicsByUPCAsync(id);
                    model = new
                    {
                        Title = "Store product brief info",
                        ProductsNumber = query.products_number,
                        Price = query.selling_price,
                        ProductName = query.product_name,
                        Characteristics = query.characteristics,
                    };
                }
                else
                {
                    var query = await _storeProductRepository.GetStoreProductPriceAndQuantityByUPCAsync(id);
                    model = new
                    {
                        Title = "Store product brief info",
                        ProductsNumber = query.products_number,
                        Price = query.selling_price,
                    };
                }
                return View("BriefInfo", model);
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message,
                };
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Route("api/store-products")]
        [JwtAuthorize]
        public async Task<string> ApiFetchAllStoreProducts()
        {
            var storeProducts = await _storeProductRepository.GetAllStoreProductsOrderedByNameAsync();
            return JsonConvert.SerializeObject(storeProducts, Formatting.Indented);
        }
    }
}
