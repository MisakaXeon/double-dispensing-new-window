using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensingMachine.Infrastracture.Models
{
    public class MedicineTakeRecord
    {
        public DateTime TakeTime { get; set; }
        public string Recipe { get; set; }
        public string Medicine { get; set; }
        public int Quantity { get; set; }
        public string Patient { get; set; }
        public int Age { get; set; }
        public string Status { get; set; }
        public string Gender { get; set; }
    }
}
