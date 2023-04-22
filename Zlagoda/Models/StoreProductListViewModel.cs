using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class StoreProductListViewModel
    {
        public string Title { get; set; } = string.Empty;
        public IEnumerable<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();
        public string? Promo { get; set; }
        public string? Sorting { get; set; }
        public dynamic Errors { get; set; } = new List<string>();
    }
}
