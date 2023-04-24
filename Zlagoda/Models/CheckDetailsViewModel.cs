using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class CheckDetailsViewModel
    {
        public string Title { get; set; } = string.Empty;
        public Check Check { get; set; } = new Check();
        public dynamic Errors { get; set; } = new List<string>();
    }
}
