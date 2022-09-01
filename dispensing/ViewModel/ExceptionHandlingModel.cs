using dispensing.CustomCommand;
using dispensing.CustomControls;
using dispensing.Expends;
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
    /// <summary>
    /// 异常护理数据模型
    /// </summary>
    public class ExceptionHandlingModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 查询命令
        /// </summary>
        public ICommand SearchCommand { get; set; }
        /// <summary>
        /// 查询到的处方信息
        /// </summary>
        public TakeMedicineInfoModel SearchedInfo { get; set; } = new TakeMedicineInfoModel()
        {
            // 模拟数据
            //Address = "北京大学医院",
            //Age = "16",
            //Date = DateTime.Now,
            //Name = "张三",
            //Serial = "0554564874",
            //Sex = "男",
            //MedicineInfos = new ObservableCollection<MedicineInfo>() {
            //    new MedicineInfo(){ Count = 5, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品1" },
            //    new MedicineInfo(){ Count = 3, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品2" },
            //    new MedicineInfo(){ Count = 1, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品3" },
            //    new MedicineInfo(){ Count = 2, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品4" },
            //    new MedicineInfo(){ Count = 4, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品5" },
            //    new MedicineInfo(){ Count = 7, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品6" },
            //    new MedicineInfo(){ Count = 8, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品7" },
            //    new MedicineInfo(){ Count = 10, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品8" },
            //}
        };

        public ExceptionHandlingModel()
        {
            Http http = new Http();
            SearchCommand = new SampleCommand(o => true, o =>
            {
                string searchInfo = o as string;
                // 做处理命令代码
                Console.WriteLine(searchInfo);
                if (searchInfo.Trim() == "")
                {
                    DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "请输入处方号", null, null, "wait"));
                    return;
                }
                SinglePresc singlePresc;
                if (!http.GetPreScription(searchInfo, out singlePresc))
                {
                    DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "查询失败\n请确认处方号是否正确", null, null, "wait"));
                    return;
                }
                SearchedInfo = new TakeMedicineInfoModel()
                {
                    Address = DosingAdministration.hospitalName,
                    Age = singlePresc.patAge,
                    TakeDate = ConvertToTime(singlePresc.enterDateTime),
                    MedicineDate = ConvertToTime(singlePresc.enterDateTime),
                    Name = singlePresc.patName,
                    Serial = singlePresc.prescriptionNo,
                    Sex = singlePresc.patGender
                };
                ObservableCollection<MedicineInfo> medicineInfos = new ObservableCollection<MedicineInfo>();
                foreach (var item in singlePresc.prescItemList)
                {
                    medicineInfos.Add(new MedicineInfo()
                    {
                        Count = item.quantity,
                        ImageUri = new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"),
                        Name = item.drugName
                    });
                }
                SearchedInfo.MedicineInfos = medicineInfos;
            });
        }
        /// <summary>
        /// 时间戳转换成标准时间
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns></returns>
        private DateTime ConvertToTime(string timeStamp)
        {
            DateTime time = DateTime.Now;
            if (string.IsNullOrEmpty(timeStamp))
            {
                return time;
            }
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(timeStamp + "0000");
                TimeSpan toNow = new TimeSpan(lTime);
                time = dtStart.Add(toNow);
            }
            catch (Exception ex)
            {
                log4net.log.Error(ex.Message);
            }
            return time;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
