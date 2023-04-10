namespace Zlagoda.Business.Entities
{
	public class CustomerCard
	{
		public string card_number { get; set; } = string.Empty;
		public string card_surname { get; set; } = string.Empty;
		public string card_name { get; set; } = string.Empty;
		public string? card_patronymic { get; set; } = null;
		public string phone_number { get; set; } = string.Empty;
		public string? city { get; set; } = null;
		public string? street { get; set; } = null;
		public string? zip_code { get; set; } = null;
		public int percent { get; set; }
	}
}
