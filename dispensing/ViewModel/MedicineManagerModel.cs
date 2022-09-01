using dispensing.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing.ViewModel
{
    /// <summary>
    /// 药品管理模型
    /// </summary>
    public class MedicineManagerModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        


        /// <summary>
        /// 每一层药槽的信息
        /// </summary>
        public ObservableCollection<LayerContent> LayerContents { get; set; } = new ObservableCollection<LayerContent>();
        /// <summary>
        /// 当前选中的层号
        /// </summary>
        public int CurrentSelectedLayerIndex { get; set; } = 0;
        /// <summary>
        /// 当前被选中的层
        /// </summary>
        public LayerContent CurrentSelectedLayerContent { get; set; }

        public MedicineManagerModel()
        {
            MockDatas();
        }
        /// <summary>
        /// 生成模拟数据
        /// </summary>
        public void MockDatas()
        {
            LayerContents.Clear();
            for (int i = 0; i < 10; i++)
            {
                var layer = new LayerContent() { LayerName = (i + 1) + "层", LayerIndex = i };
                layer.MockDatas(i);
                LayerContents.Add(layer);
            }
        }
    }
}
