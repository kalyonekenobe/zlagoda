using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class EditStoreProductViewModel
    {
        public string Title { get; set; } = string.Empty;
        public StoreProduct StoreProduct { get; set; } = new StoreProduct();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public dynamic Errors { get; set; } = new List<string>();
    }
}
