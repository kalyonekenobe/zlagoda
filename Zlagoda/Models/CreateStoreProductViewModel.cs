using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class CreateStoreProductViewModel
    {
        public string Title { get; set; } = string.Empty;
        public StoreProduct StoreProduct { get; set; } = new StoreProduct(); 
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<StoreProduct> NonPromotionalStoreProducts { get; set; } = new List<StoreProduct>();
        public string? ParentUPC { get; set; }
        public dynamic Errors { get; set; } = new List<string>();
    }
}
