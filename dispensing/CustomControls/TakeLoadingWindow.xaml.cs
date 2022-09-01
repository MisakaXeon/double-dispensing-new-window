using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace dispensing.CustomControls
{
    /// <summary>
    /// TakeLoadingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TakeLoadingWindow : Window
    {
        public TakeLoadingWindow()
        {
            InitializeComponent();
            this.Loaded += TakeLoadingWindow_Loaded;
            this.WindowState = WindowState.Maximized;

        }

        private async void TakeLoadingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(3000); 
            // 数据加载
            // 加载完成以后自动关闭欢迎页
            this.Close();   
        }
    }
}
