using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensingMachine.Infrastracture.Models
{
    public class MedicineAddRecord
    {
        public DateTime AddTime { get; set; }
        public string Operator { get; set; }
        public string Medicine { get; set; }
        public int Quantity { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Status { get; set; }
        public int Machine { get; set; }
    }
}
