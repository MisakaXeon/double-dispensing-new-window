using dispensing.CustomCommand;
using dispensing.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace dispensing.ViewModel
{
    public class RecordBookModel : INotifyPropertyChanged
    {
        sqlite m_sql = new sqlite();
        /// <summary>
        /// 查询命令
        /// </summary>
        public ICommand SearchCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 加药记录
        /// </summary>
        public ObservableCollection<MedicineOptionItem> AddList { get; set; } = new ObservableCollection<MedicineOptionItem>()
        {
            //new MedicineOptionItem()
            //{
            //    Count = 1,
            //    Date = DateTime.Now,
            //    MedicineName = "药品1",
            //    Option ="李四",
            //    PeriodDate = DateTime.Now,
            //    Serial = "123121",
            //    State = "完成"
            //},

            //new MedicineOptionItem()
            //{
            //    Count = 2,
            //    Date = DateTime.Now,
            //    MedicineName = "药品2",
            //    Option ="李四",
            //    PeriodDate = DateTime.Now,
            //    Serial = "123121",
            //    State = "完成"
            //},
        };
        /// <summary>
        /// 取药记录
        /// </summary>
        public ObservableCollection<MedicineOptionItem> GetList { get; set; } = new ObservableCollection<MedicineOptionItem>()
        {
            //new MedicineOptionItem()
            //{
            //    Count = 1,
            //    Date = DateTime.Now,
            //    MedicineName = "药品1",
            //    Option ="李四",
            //    PeriodDate = DateTime.Now,
            //    Serial = "123121",
            //    State = "完成",
            //    Age = 12,
            //    Name = "张三",
            //    PrescriptionNumber = "12121",
            //    Sex = "男"
            //}
        };


        public RecordBookModel()
        {
            m_sql.Init(1);
            SearchCommand = new SampleCommand(o => true, o =>
            {
                string searchInfo = o as string;
                // 做处理命令代码
                Console.WriteLine(searchInfo);
                if (searchInfo == "加药记录")
                {
                    AddList.Clear();
                    m_sql.QueryMedicAddRecord(out List<AddRecord> addRecords);
                    foreach (var item in addRecords)
                    {
                        AddList.Add(new MedicineOptionItem()
                        {
                            Count = item.quantity,
                            Date = DateTime.ParseExact(item.addTime, "yyyyMMdd HH:mm", System.Globalization.CultureInfo.CurrentCulture),
                            MedicineName = item.medicName,
                            Option = item.doctorName,
                            PeriodDate = DateTime.ParseExact(item.validity, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                            Serial = item.batch,
                            State = item.status
                        });
                    }
                }
                else if (searchInfo == "取药记录")
                {
                    GetList.Clear();
                    m_sql.QueryMedicGetRecord(out List<GetRecord> getRecords);
                    foreach (var item in getRecords)
                    {
                        GetList.Add(new MedicineOptionItem()
                        {
                            PrescriptionNumber = item.prescription,
                            Count = item.quantity,
                            Date = DateTime.ParseExact(item.getTime, "yyyyMMdd HH:mm", System.Globalization.CultureInfo.CurrentCulture),
                            MedicineName = item.medicName,
                            Name = item.patName,
                            Sex = item.patGender,
                            Age = item.patAge,
                            State = item.status
                        });
                    }
                }
            });
        }
    }
}
