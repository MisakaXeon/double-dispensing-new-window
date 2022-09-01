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
using System.Windows.Navigation;
using System.Windows.Shapes;
using dispensing.CustomCommand;
using dispensing.CustomControls;
using dispensing.ExDependencies;
using dispensing.Expends;

namespace dispensing
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.ShowDialog();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.WindowState = WindowState.Maximized;


            //new LoadingWindow().ShowDialog();
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("程序异常奔溃 : " + e.Exception.ToString());
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //await this.ShowMessageAsync(new DispensingDialog("知道了!", "请到柜台取药!", "取消"));
            //await this.ShowMessageAsync(new DispensingDialog("知道了!", "请到柜台取药!"));
            //DispensingDialog dialog = new DispensingDialog(null, "加载中...", icon: "boll")
            //{
            //    MaskOpacity = 0.5,
            //    MaskBrush = new SolidColorBrush(Colors.Gray),
            //    DisplayBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"))
            //};

            //await this.ShowMessageAsync(dialog);//显示

            //dialog.CloseAsync();//关闭


            //await this.ShowMessageAsync(new DispensingDialog("知道了", "取药", description: "快点吧", icon: null));
            //await this.ShowMessageAsync(new ToastDialog(false, "退货失败"));

            //PageContent.Content = // new 一个组件

            // 赋值全局键盘控件
            TextboxExDependency.FullKeyboardControl = KeyboardControl;
        }
    }
}
