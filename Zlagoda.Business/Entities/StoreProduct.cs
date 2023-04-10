﻿namespace Zlagoda.Business.Entities
{
	public class StoreProduct
	{
		public string UPC { get; set; } = string.Empty;
		public string UPC_prom { get; set; } = string.Empty;
		public int id_product { get; set; }
		public decimal selling_price { get; set; }
		public int products_number { get; set; }
		public bool promotional_product { get; set; }
	}
}
