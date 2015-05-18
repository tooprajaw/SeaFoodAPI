using BusinessLogics.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Facade
{
    public class ProductTypeFacade : IFacade<ProductType>
    {
        public ProductType Get(int id)
        {
            return new ProductType();
        }
        public List<ProductType> GetAll()
        {
            return new List<ProductType>();
        }

        public void Update(ProductType product)
        {

        }

        public void Delete(ProductType product)
        {

        }
    }
}
