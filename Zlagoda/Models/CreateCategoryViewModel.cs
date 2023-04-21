using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class CreateCategoryViewModel
    {
        public string Title { get; set; } = string.Empty;
        public Category Category { get; set; } = new Category();
        public dynamic Errors { get; set; } = new List<string>();
    }
}
