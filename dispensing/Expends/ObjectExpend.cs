using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing.Expends
{
    public static class ObjectExpend
    {

        public static T CopyFrom<T>(this T t)
        {
            Type type = typeof(T);

            var obj = (T)Activator.CreateInstance(type);

            foreach (var item in type.GetProperties())
            {
                item.SetValue(obj, item.GetValue(t));
            }
            return obj;
        }

        public static void CopyTo<T>(this T t, T target)
        {
            Type type = typeof(T);

            foreach (var item in type.GetProperties())
            {
                item.SetValue(target, item.GetValue(t));
            }
        }

        public static T CopyFromOnlyValue<T>(this T t)
        {
            Type type = typeof(T);

            var obj = (T)Activator.CreateInstance(type);

            foreach (var item in type.GetProperties())
            {
                if(item.PropertyType.IsValueType || item.PropertyType.Equals(typeof(string)))
                    item.SetValue(obj, item.GetValue(t));
            }
            return obj;
        }
    }
}
