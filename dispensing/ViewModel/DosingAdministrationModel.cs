using dispensing.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing.ViewModel
{
    public class DosingAdministrationModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 药品管理数据模型
        /// </summary>
        public MedicineManagerModel MedicineManagerModel { get; set; } = new MedicineManagerModel();
        /// <summary>
        /// 库存模型
        /// </summary>
        public StockStatisticsModel StockModel { get; set; } = new StockStatisticsModel();
        /// <summary>
        /// 异常处理模型
        /// </summary>
        public ExceptionHandlingModel ExceptionModel { get; set; } = new ExceptionHandlingModel();
        /// <summary>
        /// 记录本 数据模型
        /// </summary>
        public RecordBookModel RecordBookModel { get; set; } = new RecordBookModel();
        /// <summary>
        /// 测试数据 需要后续删除
        /// </summary>
        public LayerMedicineInfo MockItem { get; set; } = new LayerMedicineInfo()
        {
            Position = "201",
            Name = "药品测试",
            Stock = 10,
            Date = DateTime.Now,
            SerialNumber = "123456"
        };
    }
}
