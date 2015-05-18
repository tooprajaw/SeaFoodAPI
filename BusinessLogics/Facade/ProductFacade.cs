using BusinessLogics.BusinessObjects;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Facade
{
    public class ProductFacade : IFacade<Product>
    {
        ProductDao dao;
        public ProductFacade()
        {
            dao = new ProductDao();
        }
        public Product Get(int id)
        {
            return dao.GetProduct(new Product { ProductID = id });
        }
        public List<Product> GetAll()
        {
            return dao.GetAllProducts();
        }
        public List<Product> GetAllByProductType(int productTypeID)
        {
            return dao.GetAllProducts(new Product { ProductTypeID = productTypeID });
        }

        public void Update(Product product)
        {

        }

        public void Delete(Product product)
        {

        }
    }
}
