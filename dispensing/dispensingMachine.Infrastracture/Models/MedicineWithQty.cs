using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensingMachine.Infrastracture.Models
{
    public class MedicineWithQty : Medicine
    {
        private int quantity;
        public int Quantity { get => quantity; set {
                quantity = value;
                OnPropertyChanged("Quantity");
            }}
    }
}
