using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    interface IFacade<T> where T : new()
    {
        T Get(int id);
        List<T> GetAll();
        void Update(T t);
        void Delete(T t);
    }
}
