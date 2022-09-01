using dispensing.CustomCommand;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// IndexPage.xaml 的交互逻辑
    /// </summary>
    public partial class IndexPage : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 设置命令
        /// </summary>
        public ICommand SettingCommand { get; set; }
        /// <summary>
        /// 退出命令
        /// </summary>
        public ICommand ExitCommand { get; set; }
        /// <summary>
        /// 回退命令
        /// </summary>
        public ICommand BackCommand { get; set; }

        public IndexPage()
        {
            InitializeComponent();

            SettingCommand = new SampleCommand(o => true, o =>
            {
                // TODO 点击设置之后触发的内容

            });
            // 关闭退出
            ExitCommand = new SampleCommand(o => true, o => {
                //App.Current.MainWindow.Close();
                App.Current.Shutdown();
            });

            BackCommand = new SampleCommand(o => true, o => {
                IndexPageControl.BackCommandEvent();
            });

            this.Loaded += IndexPage_Loaded;
        }

        private void IndexPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
