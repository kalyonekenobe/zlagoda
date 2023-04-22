using Microsoft.AspNetCore.Mvc;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Models;

namespace Zlagoda.Controllers
{
    public class StoreProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreProductRepository _storeProductRepository;

        public StoreProductController(IProductRepository productRepository, IStoreProductRepository storeProductRepository)
        {
            _productRepository = productRepository;
            _storeProductRepository = storeProductRepository;
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
                Errors = TempData["Errprs"] ?? new List<string>(),
            }; 
            return View("List", model);
        }
    }
}
