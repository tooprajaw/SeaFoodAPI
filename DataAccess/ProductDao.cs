using BusinessLogics.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductDao
    {
        private static readonly string sp_GetProduct = "sp_GetProduct";
        private static readonly string sp_GetAllProducts = "sp_GetAllProducts";
        public Product GetProduct(Product product)
        {
            return Db.Read<Product>(sp_GetProduct, product);
        }
        public List<Product> GetAllProducts()
        {
            return Db.ReadList<Product>(sp_GetAllProducts);
        }
        public List<Product> GetAllProducts(Product product)
        {
            return Db.ReadList<Product>(sp_GetAllProducts, product);
        }
    }
}
