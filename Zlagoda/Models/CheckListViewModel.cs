using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class CheckListViewModel
    {
        public string Title { get; set; } = string.Empty;
        public IEnumerable<Check> Checks { get; set; } = new List<Check>();
        public dynamic Errors { get; set; } = new List<string>();
    }
}
