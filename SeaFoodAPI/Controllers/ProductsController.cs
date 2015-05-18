using BusinessLogics.BusinessObjects;
using BusinessLogics.Facade;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SeaFoodAPI.Controllers
{
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        public ProductsController()
        {

        }

        public IHttpActionResult GetProduct(int id)
        {
            try
            {
                ProductFacade pf = new ProductFacade();
                Product p = pf.Get(id);
                if (p == null)
                {
                    return NotFound();
                }
                return Ok<Product>(p);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IHttpActionResult GetAllProducts()
        {
            try
            {
                ProductFacade pf = new ProductFacade();
                List<Product> pList = pf.GetAll();
                if (pList == null)
                {
                    return NotFound();
                }
                return Ok<List<Product>>(pList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("GetAllProductsByType")]
        public IHttpActionResult GetAllProductsByType(JObject jsonData)
        {
            try
            {
                dynamic json = jsonData;
                int productTypeID = Convert.ToInt32(json.ProductTypeID);
                ProductFacade pf = new ProductFacade();
                List<Product> pList = pf.GetAllByProductType(productTypeID);
                if (pList == null)
                {
                    return NotFound();
                }
                return Ok<List<Product>>(pList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
