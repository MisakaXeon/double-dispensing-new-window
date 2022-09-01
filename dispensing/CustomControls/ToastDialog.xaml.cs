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

namespace dispensing.CustomControls
{
    /// <summary>
    /// ToastDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ToastDialog : CustomDialogControl
    {
        /// <summary>
        /// 提示内容
        /// </summary>
        public string ToastContent { get; set; }
        /// <summary>
        /// 是否是ok的提示
        /// </summary>
        public bool? IsOk { get; set; }
        /// <summary>
        /// 显示时常
        /// </summary>
        public long WaitMillisecond { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isok">是否ok</param>
        /// <param name="toast">提示内容</param>
        /// <param name="millisecond">显示时间,毫秒</param>
        public ToastDialog(bool? isok, string toast, long millisecond = 2000, int width = 180, int height = 180)
        {
            InitializeComponent();
            this.Loaded += ToastDialog_Loaded;
            this.ToastContent = toast;
            this.IsOk = isok;
            MaskOpacity = 0;
            this.Width = width;
            this.Height = height;   
            this.WaitMillisecond = millisecond;
        }

        private async void ToastDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
            await Task.Delay(TimeSpan.FromMilliseconds(this.WaitMillisecond));
            this.CloseAsync();
        }
    }
}
