using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing.Expends
{
    public static class EnumExpend
    {

        public static T Converter<T>(this string str, bool isCaseLower = true) where T : struct
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (isCaseLower)
                {
                    if (item.ToString().Equals(str))
                        return item;
                }
                else
                {
                    if (item.ToString().Equals(str, StringComparison.OrdinalIgnoreCase))
                        return item;
                }
            }
            return default(T);
        }
    }
}
