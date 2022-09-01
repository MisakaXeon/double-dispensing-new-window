using dispensing.CustomCommand;
using dispensing.Expends;
using dispensing.ViewModel;
using dispensing.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dispensing.CustomControls
{
    /// <summary>
    /// TakeMedicineInfoPage.xaml 的交互逻辑
    /// </summary>
    public partial class TakeMedicineInfoPage : UserControl
    {
        /// <summary>
        /// 退出命令
        /// </summary>
        public ICommand ExitCommand { get; set; }
        /// <summary>
        /// 确定取药命令
        /// </summary>
        public ICommand TakeMedicineCommand { get; set; }

        public TakeMedicineInfoModel Model = new TakeMedicineInfoModel()
        {
            Address = "北京大学医院",
            Age = "16",
            MedicineDate = DateTime.Now,
            TakeDate = DateTime.Now,
            Name = "张三",
            Serial = "0554564874",
            Sex = "男",
            MedicineInfos = new ObservableCollection<MedicineInfo>() { 
                new MedicineInfo(){ Count = 5, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品1" },
                new MedicineInfo(){ Count = 3, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品2" },
                new MedicineInfo(){ Count = 1, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品3" },
                new MedicineInfo(){ Count = 2, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品4" },
                new MedicineInfo(){ Count = 4, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品5" },
                new MedicineInfo(){ Count = 7, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品6" },
                new MedicineInfo(){ Count = 8, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品7" },
                new MedicineInfo(){ Count = 10, ImageUri=new Uri("https://img2.baidu.com/it/u=1146725618,4845454&fm=253&fmt=auto&app=138&f=JPEG?w=600&h=500"), Name = "药品8" },
            }
        };

        public TakeMedicineInfoPage()
        {
            InitializeComponent();
            ExitCommand = new SampleCommand(o => true, async o => {
                await this.ShowMessageAsync(new DispensingDialog("确定", "确定要退出系统吗!", "取消", "退出后将回到取药首页", icon: null) {
                    CloseAction = res => {
                        if(res == true)
                        {
                            //log4net.log.Info("写入确认状态");
                            TestProcedure.uIConfirmState[0] = true;
                            TestProcedure.uIConfirmState[1] = false;
                            this.Visibility = Visibility.Collapsed; ;
                        }
                        //log4net.log.Info("写入确认状态1");
                    }
                });
            });

            TakeMedicineCommand = new SampleCommand(o => true, o => {
                //确定取药 命令 TODO
                TestProcedure.uIConfirmState[0] = true;
                TestProcedure.uIConfirmState[1] = true;
                //this.Visibility = Visibility.Collapsed;
                //string str = "";
            });
            this.Loaded += TakeMedicineInfoPage_Loaded;
            
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            this.CloseKeyboard();
        }


        private void TakeMedicineInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Model;
            
        }

    }
}
