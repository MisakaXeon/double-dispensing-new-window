using dispensing.CustomCommand;
using dispensing.CustomControls;
using dispensing.ExDependencies;
using dispensing.Expends;
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

namespace dispensing
{
    /// <summary>
    /// IndexMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IndexMainWindow : Window
    {
        public IndexMainWindow()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            // 欢迎页
            TakeLoadingWindow takeLoadingWindow = new TakeLoadingWindow();
            takeLoadingWindow.ShowDialog();
            InitializeComponent();
            this.Loaded += IndexMainWindow_Loaded;
            //this.WindowState = WindowState.Maximized;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("程序异常奔溃 : " + e.Exception.ToString());
        }

        private void IndexMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.ShowMessageAsync(new DispensingDialog("知道了", "取药", description: "快点吧", icon: null));
            //this.ShowToastDialogAsync(new DispensingDialog("知道了", "取药"));
            //this.ShowMessageAsync(new ToastDialog(false, "退货失败"));

            // 赋值全局键盘控件
            TextboxExDependency.FullKeyboardControl = KeyboardControl;
        }
    }
}
