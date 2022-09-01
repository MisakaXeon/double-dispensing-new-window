using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing.CustomConverters.Items
{
    public abstract class AnyConvertItem
    {
        public object From { get; set; }
        public object To { get; set; }
    }

    public class AnyItem : AnyConvertItem
    {

    }

    public class AnyOtherItem : AnyConvertItem
    {
    }
}
