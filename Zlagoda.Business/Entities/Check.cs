namespace Zlagoda.Business.Entities
{
	public class Check
	{
		public string check_number { get; set; } = string.Empty;
		public string id_employee { get; set; } = string.Empty;
		public string? card_number { get; set; } = null;
		public DateTime print_date { get; set; }
		public decimal sum_total { get; set; }
		public decimal vat { get; set; }

		public Employee employee { get; set; } = new Employee();
		public CustomerCard? customer_card { get; set; } = null;
		public List<Sale> sales { get; set; } = new List<Sale>();
	}
}
