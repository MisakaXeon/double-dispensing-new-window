using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensingMachine.Infrastracture.Models
{
    public class ErrorData
    {
        public DateTime ErrorDate { get; set; }
        public string Recipe { get; set; }
        public string Name { get; set; }
        public string ErrorResult { get; set; }
        public string Status { get; set; }
    }
}
