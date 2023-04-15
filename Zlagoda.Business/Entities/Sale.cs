namespace Zlagoda.Business.Entities
{
	public class Sale
	{
		public string UPC { get; set; } = string.Empty;
		public string check_number { get; set; } = string.Empty;
		public int product_number { get; set; } 
		public decimal selling_price { get; set; }

		public StoreProduct storeProduct { get; set; } = new StoreProduct();
	}
}
