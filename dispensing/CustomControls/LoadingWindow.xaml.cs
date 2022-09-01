using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// LoadingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
            this.Loaded += LoadingWindow_Loaded;
            this.WindowState = WindowState.Maximized;

        }

        private async void  LoadingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(3000);
            // 数据加载


            // 关闭
            this.Close();
        }
    }
}
