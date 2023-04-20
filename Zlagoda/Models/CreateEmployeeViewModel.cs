using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class CreateEmployeeViewModel
    {
        public string Title { get; set; } = string.Empty;
        public Employee Employee { get; set; } = new Employee();
        public List<string> Errors = new List<string>();
    }
}
