using dispensing.ExDependencies;
using dispensing.Expends;
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
    /// VerifyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyDialog : CustomDialogControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 验证命令
        /// </summary>
        public ICommand VertifyCommand { get; set; }
        /// <summary>
        /// 键盘
        /// </summary>
        public FullKeyboardControl FullKeyboardControl { get; set; }

        public VerifyDialog()
        {
            InitializeComponent();
            this.Loaded += VerifyDialog_Loaded;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifyDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = this.FindParentVisiual<Grid>();
            foreach (var item in grid.Children)
            {
                if (item is FullKeyboardControl)
                {
                    //(item as FullKeyboardControl).Visibility = Visibility.Visible;
                    FullKeyboardControl = item as FullKeyboardControl;
                }
            }
            FullKeyboardControl.Visibility = Visibility.Collapsed;
            this.DataContext = this;
        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FullKeyboardControl.Visibility = Visibility.Visible;
        }
    }
}
