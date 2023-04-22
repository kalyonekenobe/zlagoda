using Zlagoda.Business.Entities;
namespace Zlagoda.Models
{
    public class ProductListViewModel  {
        public string Title { get; set; } = string.Empty; 
        public IEnumerable<Product> Products { get; set; } = new List<Product>(); 
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public string? search { get; set; }
        public dynamic Errors { get; set; } = new List<string>();
    }
}
