using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class CreateCheckViewModel
    {
        public string Title { get; set; } = string.Empty;
        public IEnumerable<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();
        public IEnumerable<CustomerCard> Clients { get; set; } = new List<CustomerCard>(); 
        public Check Check { get; set; } = new Check();
        public dynamic Errors { get; set; } = new List<string>();
    }
}
