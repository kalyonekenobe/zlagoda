using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class EditProductViewModel
    {
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public Product Product { get; set; } = new Product();
        public dynamic Errors { get; set; } = new List<string>();
    }
}
