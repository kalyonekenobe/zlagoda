namespace Zlagoda.Business.Entities
{
	public class Employee
	{
		public string id_employee { get; set; } = string.Empty;
		public string empl_surname { get; set; } = string.Empty;
		public string empl_name { get; set; } = string.Empty;
		public string? empl_patronymic { get; set; } = null;
		public string empl_role { get; set; } = string.Empty;
		public decimal salary { get; set; }
		public DateTime date_of_birth { get; set; }
		public DateTime date_of_start { get; set; }
		public string phone_number { get; set; } = string.Empty;
		public string city { get; set; } = string.Empty;
		public string street { get; set; } = string.Empty;
		public string zip_code { get; set; } = string.Empty;
	}
}
