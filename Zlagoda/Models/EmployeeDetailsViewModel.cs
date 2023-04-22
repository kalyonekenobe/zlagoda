using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class EmployeeDetailsViewModel
    {
        public string Title { get; set; } = string.Empty;
        public Employee Employee { get; set; } = new Employee();
    }
}
