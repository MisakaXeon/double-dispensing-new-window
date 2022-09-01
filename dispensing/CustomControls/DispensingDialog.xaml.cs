using dispensing.CustomCommand;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dispensing.CustomControls
{
    /// <summary>
    /// DispensingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DispensingDialog : CustomDialogControl
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 标题下的描述文字
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 确认的文本
        /// </summary>
        public string OkMessage { get; set; }
        /// <summary>
        /// 取消的文本
        /// </summary>
        public string CancelMessage { get; set; }
        /// <summary>
        /// 自定义显示背景颜色
        /// </summary>
        public Brush DisplayBackground { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 确认按钮
        /// </summary>
        public ICommand SureCommand { get; set; }
        /// <summary>
        /// 取消按钮
        /// </summary>
        public ICommand CancelCommand { get; set; }
        /// <summary>
        /// 是否是没有按钮区域
        /// </summary>
        public bool IsNoBtnBar { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="window"></param>
        /// <param name="okMessage"></param>
        /// <param name="title"></param>
        /// <param name="cancelMessage">为空不显示</param>
        /// <param name="description">描述文字</param>
        /// <param name="icon">信息：lament;加载：loading;等待：wait</param>
        /// <param name="cancelMessage"></param>
        /// <param name="description">描述文字</param>
        /// <param name="icon"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public DispensingDialog(string okMessage, string title, string cancelMessage = null, string description = null, string icon = "lament", int width = 600, int height = 348)
        {
            InitializeComponent();
            Loaded += DispensingDialog_Loaded;
            this.OkMessage = okMessage;
            this.Title = title;
            this.Width = width;
            this.Height = height;
            this.CancelMessage = cancelMessage;
            this.Description = description;
            this.Icon = icon;

            IsNoBtnBar = okMessage == null && cancelMessage == null;
            

            SureCommand = new SampleCommand(o => true, o => {
                try
                {
                    if (IsClosed) return;
                    CloseAction?.Invoke(true);
                }
                catch (Exception)
                {
                }
                finally
                {
                    AutoReset.Set();
                }
            });
            CancelCommand = new SampleCommand(o => true, o => {
                try
                {
                    if (IsClosed) return;
                    CloseAction?.Invoke(false);
                }
                catch (Exception)
                {
                }
                finally
                {
                    AutoReset.Set();
                }
            });
        }
        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispensingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DisplayBackground != null)
            {
                this.BackgroundBorder.Background = DisplayBackground;
            }
            this.DataContext = this;
        }

    }
}
