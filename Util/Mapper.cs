using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class Mapper
    {
        public static void Map<T, D>(T source, D destination) where D : new()
        {
            if (source == null)
                return;

            if (destination == null)
            {
                destination = new D();
            }

            foreach (PropertyInfo property in source.GetType().GetProperties())
            {
                var exists = destination.GetType().GetProperties().FirstOrDefault(p => p.Name == property.Name) != null;
                if (exists)
                {
                    destination.GetType().GetProperty(property.Name).SetValue(destination, property.GetValue(source, null), null);
                }
            }
        }

        public static D Map<T, D>(T source) where D : new()
        {

            D destination = new D();

            if (source != null)
            {
                foreach (PropertyInfo property in source.GetType().GetProperties())
                {
                    var exists = destination.GetType().GetProperties().FirstOrDefault(p => p.Name == property.Name) != null;
                    if (exists)
                    {
                        destination.GetType().GetProperty(property.Name).SetValue(destination, property.GetValue(source, null), null);
                    }
                }
            }

            return destination;
        }

        public static List<D> Map<T, D>(List<T> source) where D : new()
        {
            List<D> list = new List<D>();

            foreach (var item in source)
            {
                D d = Map<T, D>(item);
                list.Add(d);
            }

            return list;
        }

    }
}
