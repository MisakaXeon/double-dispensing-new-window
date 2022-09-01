using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensingMachine.Infrastracture.Models
{
    public class MedicineInBox : MedicineWithQty
    {
        // 药品序号
        private string itemId;
        // 层
        private int level;
        private string boxId;
        // 批号
        private string batchNumber;

        public string expireDate;
        private int x;
        private int y;
        private int z;
        private string mark;
        private int machine;

        // 药品序号
        public string ItemId
        {
            get => itemId; set
            {
                itemId = value;
                OnPropertyChanged("ItemId");
            }
        }
        // 层
        public int Level
        {
            get => level; set
            {
                level = value;
                OnPropertyChanged("Level");
            }
        }
        //机器号
        public int Machine
        {
            get => machine; set
            {
                machine = value;
                OnPropertyChanged("Machine");
            }
        }
        public string BoxId
        {
            get => boxId; set
            {
                boxId = value;
                OnPropertyChanged("BoxId");
            }
        }
        // 批号
        public string BatchNumber
        {
            get => batchNumber; set
            {
                batchNumber = value;
                OnPropertyChanged("BatchNumber");
            }
        }

        public DateTime ExpireDate
        {
            //get => DateTime.ParseExact(expireDate, "yyyyMMdd", null); set
            get
            {
                DateTime ret;
                if (!DateTime.TryParseExact(expireDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ret))
                    ret = DateTime.Now;
                return ret;
            }
            set
            {
                expireDate = value.ToString("yyyyMMdd");
                OnPropertyChanged("ExpireDate");
            }
        }
        public int X
        {
            get => x; set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }
        public int Y
        {
            get => y; set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }
        public int Z
        {
            get => z; set
            {
                z = value;
                OnPropertyChanged("Z");
            }
        }
        public string Mark
        {
            get => mark; set
            {
                mark = value;
                OnPropertyChanged("Mark");
            }
        }

        public string Size
        {
            get => string.Format("{0}*{1}*{2}", X, Y, Z);
        }
    }
}
