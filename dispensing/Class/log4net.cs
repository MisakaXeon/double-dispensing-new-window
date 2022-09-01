using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;


namespace dispensing
{
    class log4net
    {
        public static ILog log = LogManager.GetLogger("test");
    }

}
