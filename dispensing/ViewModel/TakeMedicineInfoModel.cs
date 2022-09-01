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
    public class TakeMedicineInfoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
#nullable enable
        /// <summary>
        /// 年龄
        /// </summary>
        public string? Age { get; set; }
#nullable disable
        /// <summary>
        /// 处方编号
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// 开设处方地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 处方日期
        /// </summary>
        public DateTime MedicineDate { get; set; }
        /// <summary>
        /// 取药日期
        /// </summary>
        public DateTime TakeDate { get; set; }
        /// <summary>
        /// 退出命令
        /// </summary>
        public ICommand ExitCommand { get; set; }
        /// <summary>
        /// 取药命令
        /// </summary>
        public ICommand TakeMedicineCommand { get; set; }
        /// <summary>
        /// 药品列表
        /// </summary>
        public ObservableCollection<MedicineInfo> MedicineInfos { get; set; } = new ObservableCollection<MedicineInfo>();
        /// <summary>
        /// 药品种类数量
        /// </summary>
        public int Classes => MedicineInfos.Count;
        /// <summary>
        /// 所有药品总件数
        /// </summary>
        public int TotalCount => MedicineInfos.Sum(p => p.Count);

        public TakeMedicineInfoModel()
        {
            ExitCommand = new SampleCommand(o => true, o =>
            {
                // 处理退出逻辑
            });

            TakeMedicineCommand = new SampleCommand(o => true, o =>
            {
                // 处理取药逻辑
            });
        }
    }
}
