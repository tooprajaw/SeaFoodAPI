using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.BusinessObjects
{
    public class Product
    {
        public int ProductID { get; set; }
        public int ProductTypeID { get; set; }
        public string Name { get; set; }
        public string ThaiName { get; set; }
        public string Description { get; set; }
        public string RawImage { get; set; }
        public decimal Price { get; set; }
    }
}
