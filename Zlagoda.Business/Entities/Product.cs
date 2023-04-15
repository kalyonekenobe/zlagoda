namespace Zlagoda.Business.Entities
{
	public class Product
	{
		public int id_product { get; set; }
		public int category_number { get; set; }
		public string product_name { get; set; } = string.Empty;
		public string characteristics { get; set; } = string.Empty;

		public Category category { get; set; } = new Category();
	}
}
