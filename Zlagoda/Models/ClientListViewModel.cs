using Microsoft.AspNetCore.Mvc;
using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class ClientListViewModel
    {
        public string Title { get; set; } = string.Empty;
        public IEnumerable<CustomerCard> Clients { get; set; } = new List<CustomerCard>();
        public string? surname { get; set; } = null;
        public int? percent { get; set; } = null;
        public dynamic Errors { get; set; } = new List<string>();
    }
}
