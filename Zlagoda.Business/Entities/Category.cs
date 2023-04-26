namespace Zlagoda.Business.Entities
{
	public class Category
	{
		public int category_number { get; set; }
		public string category_name { get; set; } = string.Empty;
		public int? store_products_number { get; set; } = null;
	}
}
