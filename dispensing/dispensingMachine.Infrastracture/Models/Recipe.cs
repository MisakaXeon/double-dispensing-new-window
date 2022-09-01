using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensingMachine.Infrastracture.Models
{
    public enum Gender
    {
        male,
        female,
    }

    public class Recipe
    {
        // 处方编号
        public string Code { get; set; }
        // 患者
        public string Patient { get; set; }
        // 性别 
        public Gender Gender { get; set; }
        // 年龄
        public int Age { get; set; }
        // 医院
        public string Hospital { get; set; }

        public DateTime Date { get; set; }

        public List<MedicineWithQty> Medicines { get; set; }
    }
}
