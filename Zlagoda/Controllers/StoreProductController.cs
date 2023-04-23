using Microsoft.AspNetCore.Mvc;
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
        [JwtAuthorize]
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
    }
}
