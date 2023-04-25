using Dapper;
using System.Data.SqlClient;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Business.Repositories
{
    public class StoreProductRepository : IStoreProductRepository
    {
        private readonly string _connectionString;

        public StoreProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<StoreProduct> CreateStoreProductAsync(StoreProduct storeProduct)
        {
            if (!await CanCreateStoreProduct(storeProduct))
            {
                throw new Exception("Cannot create more than 2 store products for the same product!");
            }
            string query = @"INSERT INTO Store_Product
                             (UPC, UPC_prom, id_product, selling_price, products_number, promotional_product)
                             VALUES
                             (@UPC, @UPCProm, @IdProduct, @SellingPrice, @ProductsNumber, @PromotionalProduct)";
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    UPC = storeProduct.UPC,
                    UPCProm = storeProduct.UPC_prom,
                    IdProduct = storeProduct.id_product,
                    SellingPrice = storeProduct.selling_price,
                    ProductsNumber = storeProduct.products_number,
                    PromotionalProduct = storeProduct.promotional_product,
                });

                if (affectedRows == 0)
                {
                    throw new Exception("Store product creation error!");
                }

                return storeProduct;
            }
        }

        public async Task<StoreProduct> DeleteStoreProductAsync(StoreProduct storeProduct)
        {
            string query = @"DELETE
                             FROM Store_Product
                             WHERE UPC=@UPC";
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    UPC = storeProduct.UPC,
                });

                if (affectedRows == 0)
                {
                    throw new Exception("Store product deletion error!");
                }

                return storeProduct;
            }
        }

        public async Task<IEnumerable<StoreProduct>> GetAllNonPromotionalStoreProductsOrderedByNameThenByQuantityAsync()
        {
            string query = @"SELECT *
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product 
                             INNER JOIN Category C
                             ON P.category_number=C.category_number
                             WHERE promotional_product=0
                             ORDER BY product_name ASC, products_number ASC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<dynamic>(query);
                List<StoreProduct> storeProducts = new List<StoreProduct>();
                foreach (var resultItem in result)
                {
                    storeProducts.Add(new StoreProduct
                    {
                        UPC = resultItem.UPC,
                        UPC_prom = resultItem.UPC_prom,
                        id_product = resultItem.id_product,
                        selling_price = resultItem.selling_price,
                        products_number = resultItem.products_number,
                        promotional_product = resultItem.promotional_product,
                        product = new Product
                        {
                            id_product = resultItem.id_product,
                            category_number = resultItem.category_number,
                            product_name = resultItem.product_name,
                            characteristics = resultItem.characteristics,
                            category = new Category
                            {
                                category_number = resultItem.category_number,
                                category_name = resultItem.category_name,
                            },
                        },
                    });
                }

                return storeProducts;
            }
        }

        public async Task<IEnumerable<StoreProduct>> GetAllNonPromotionalStoreProductsOrderedByQuantityThenByNameAsync()
        {
            string query = @"SELECT *
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product 
                             INNER JOIN Category C
                             ON P.category_number=C.category_number
                             WHERE promotional_product=0
                             ORDER BY products_number ASC, product_name ASC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<dynamic>(query);
                List<StoreProduct> storeProducts = new List<StoreProduct>();
                foreach (var resultItem in result)
                {
                    storeProducts.Add(new StoreProduct
                    {
                        UPC = resultItem.UPC,
                        UPC_prom = resultItem.UPC_prom,
                        id_product = resultItem.id_product,
                        selling_price = resultItem.selling_price,
                        products_number = resultItem.products_number,
                        promotional_product = resultItem.promotional_product,
                        product = new Product
                        {
                            id_product = resultItem.id_product,
                            category_number = resultItem.category_number,
                            product_name = resultItem.product_name,
                            characteristics = resultItem.characteristics,
                            category = new Category
                            {
                                category_number = resultItem.category_number,
                                category_name = resultItem.category_name,
                            },
                        },
                    });
                }

                return storeProducts;
            }
        }

        public async Task<IEnumerable<StoreProduct>> GetAllPromotionalStoreProductsOrderedByNameThenByQuantityAsync()
        {
            string query = @"SELECT *
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product 
                             INNER JOIN Category C
                             ON P.category_number=C.category_number
                             WHERE promotional_product=1
                             ORDER BY product_name ASC, products_number ASC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<dynamic>(query);
                List<StoreProduct> storeProducts = new List<StoreProduct>();
                foreach (var resultItem in result)
                {
                    storeProducts.Add(new StoreProduct
                    {
                        UPC = resultItem.UPC,
                        UPC_prom = resultItem.UPC_prom,
                        id_product = resultItem.id_product,
                        selling_price = resultItem.selling_price,
                        products_number = resultItem.products_number,
                        promotional_product = resultItem.promotional_product,
                        product = new Product
                        {
                            id_product = resultItem.id_product,
                            category_number = resultItem.category_number,
                            product_name = resultItem.product_name,
                            characteristics = resultItem.characteristics,
                            category = new Category
                            {
                                category_number = resultItem.category_number,
                                category_name = resultItem.category_name,
                            },
                        },
                    });
                }

                return storeProducts;
            }
        }

        public async Task<IEnumerable<StoreProduct>> GetAllPromotionalStoreProductsOrderedByQuantityThenByNameAsync()
        {
            string query = @"SELECT *
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product 
                             INNER JOIN Category C
                             ON P.category_number=C.category_number
                             WHERE promotional_product=1
                             ORDER BY products_number ASC, product_name ASC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<dynamic>(query);
                List<StoreProduct> storeProducts = new List<StoreProduct>();
                foreach (var resultItem in result)
                {
                    storeProducts.Add(new StoreProduct
                    {
                        UPC = resultItem.UPC,
                        UPC_prom = resultItem.UPC_prom,
                        id_product = resultItem.id_product,
                        selling_price = resultItem.selling_price,
                        products_number = resultItem.products_number,
                        promotional_product = resultItem.promotional_product,
                        product = new Product
                        {
                            id_product = resultItem.id_product,
                            category_number = resultItem.category_number,
                            product_name = resultItem.product_name,
                            characteristics = resultItem.characteristics,
                            category = new Category
                            {
                                category_number = resultItem.category_number,
                                category_name = resultItem.category_name,
                            },
                        },
                    });
                }

                return storeProducts;
            }
        }

        public async Task<IEnumerable<StoreProduct>> GetAllStoreProductsOrderedByNameAsync()
        {
            string query = @"SELECT *
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product 
                             INNER JOIN Category C
                             ON P.category_number=C.category_number
                             ORDER BY product_name ASC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<dynamic>(query);
                List<StoreProduct> storeProducts = new List<StoreProduct>();
                foreach (var resultItem in result)
                {
                    storeProducts.Add(new StoreProduct
                    {
                        UPC = resultItem.UPC,
                        UPC_prom = resultItem.UPC_prom,
                        id_product = resultItem.id_product,
                        selling_price = resultItem.selling_price,
                        products_number = resultItem.products_number,
                        promotional_product = resultItem.promotional_product,
                        product = new Product
                        {
                            id_product = resultItem.id_product,
                            category_number = resultItem.category_number,
                            product_name = resultItem.product_name,
                            characteristics = resultItem.characteristics,
                            category = new Category
                            {
                                category_number = resultItem.category_number,
                                category_name = resultItem.category_name,
                            },
                        },
                    });
                }

                return storeProducts;
            }
        }

        public async Task<IEnumerable<StoreProduct>> GetAllStoreProductsOrderedByQuantityAsync()
        {
            string query = @"SELECT *
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product 
                             INNER JOIN Category C
                             ON P.category_number=C.category_number
                             ORDER BY products_number ASC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<dynamic>(query);
                List<StoreProduct> storeProducts = new List<StoreProduct>();
                foreach (var resultItem in result)
                {
                    storeProducts.Add(new StoreProduct
                    {
                        UPC = resultItem.UPC,
                        UPC_prom = resultItem.UPC_prom,
                        id_product = resultItem.id_product,
                        selling_price = resultItem.selling_price,
                        products_number = resultItem.products_number,
                        promotional_product = resultItem.promotional_product,
                        product = new Product
                        {
                            id_product = resultItem.id_product,
                            category_number = resultItem.category_number,
                            product_name = resultItem.product_name,
                            characteristics = resultItem.characteristics,
                            category = new Category
                            {
                                category_number = resultItem.category_number,
                                category_name = resultItem.category_name,
                            },
                        },
                    });
                }

                return storeProducts;
            }
        }

        public async Task<StoreProduct> GetStoreProductByUPCAsync(string upc)
        {
            string query = @"SELECT *
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product 
                             INNER JOIN Category C
                             ON P.category_number=C.category_number
                             WHERE SP.UPC=@UPC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync(query, new
                {
                    UPC = upc,
                });

                if (result is null)
                {
                    throw new Exception("Error when fetching store product by UPC!");
                }

                var storeProduct = new StoreProduct
                {
                    UPC = result.UPC,
                    UPC_prom = result.UPC_prom,
                    id_product = result.id_product,
                    selling_price = result.selling_price,
                    products_number = result.products_number,
                    promotional_product = result.promotional_product,
                    product = new Product
                    {
                        id_product = result.id_product,
                        category_number = result.category_number,
                        product_name = result.product_name,
                        characteristics = result.characteristics,
                        category = new Category
                        {
                            category_number = result.category_number,
                            category_name = result.category_name,
                        },
                    },
                };

                return storeProduct;
            }
        }

        public async Task<StoreProduct?> GetStoreProductNonPromotionalByUPCAsync(string upc)
        {
            string query = @"SELECT *
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product 
                             INNER JOIN Category C
                             ON P.category_number=C.category_number
                             WHERE SP.UPC_prom=@UPC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync(query, new
                {
                    UPC = upc,
                });

                if (result is null)
                {
                    return result;
                }

                var storeProduct = new StoreProduct
                {
                    UPC = result.UPC,
                    UPC_prom = result.UPC_prom,
                    id_product = result.id_product,
                    selling_price = result.selling_price,
                    products_number = result.products_number,
                    promotional_product = result.promotional_product,
                    product = new Product
                    {
                        id_product = result.id_product,
                        category_number = result.category_number,
                        product_name = result.product_name,
                        characteristics = result.characteristics,
                        category = new Category
                        {
                            category_number = result.category_number,
                            category_name = result.category_name,
                        },
                    },
                };

                return storeProduct;
            }
        }

        public async Task<dynamic> GetStoreProductPriceAndQuantityByUPCAsync(string upc)
        {
            string query = @"SELECT products_number, selling_price
                             FROM Store_Product
                             WHERE UPC=@UPC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync(query, new
                {
                    UPC = upc,
                });

                if (result is null)
                {
                    throw new Exception("Error when fetching store product quantity and price!");
                }

                return result;
            }
        }

        public async Task<dynamic> GetStoreProductPriceQuantityNameAndCharacteristicsByUPCAsync(string upc)
        {
            string query = @"SELECT selling_price, products_number, product_name, characteristics
                             FROM Store_Product SP
                             INNER JOIN Product P
                             ON SP.id_product=P.id_product
                             WHERE UPC=@UPC";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync(query, new
                {
                    UPC = upc,
                });

                if (result is null)
                {
                    throw new Exception("Error when fetching store product price, quantity, name and characteristics!");
                }

                return result;
            }
        }

        public async Task<StoreProduct> UpdateStoreProductAsync(StoreProduct storeProduct)
        {
            string query = @"UPDATE Store_Product
                             SET UPC_prom=@UPCProm, id_product=@IdProduct, selling_price=@SellingPrice, 
                             products_number=@ProductsNumber, promotional_product=@PromotionalProduct
                             WHERE UPC=@UPC";
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    UPCProm = storeProduct.UPC_prom,
                    IdProduct = storeProduct.id_product,
                    SellingPrice = storeProduct.selling_price,
                    ProductsNumber = storeProduct.products_number,
                    PromotionalProduct = storeProduct.promotional_product,
                    UPC = storeProduct.UPC,
                });

                if (affectedRows == 0)
                {
                    throw new Exception("Store product updating error!");
                }

                return storeProduct;
            }
        }

        private async Task<bool> CanCreateStoreProduct(StoreProduct storeProduct)
        {
            string query = @"(SELECT COUNT(*) FROM Store_Product WHERE id_product=@IdProduct AND promotional_product=@PromotionalProduct)";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new
                {
                    IdProduct = storeProduct.id_product,
                    PromotionalProduct = storeProduct.promotional_product,
                });
                return result == 0;
            }
        }
    }
}
