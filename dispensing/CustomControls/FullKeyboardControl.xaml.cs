using dispensing.Enums;
using dispensing.Expends;
using dispensing.ViewModel;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsApiLib.API;

namespace dispensing.CustomControls
{
    /// <summary>
    /// FullKeyboardControl.xaml 的交互逻辑
    /// </summary>
    public partial class FullKeyboardControl : UserControl
    {
        /// <summary>
        /// 数据模型
        /// </summary>
        public KeyboardControlModel Model { get; set; } = new KeyboardControlModel();
        /// <summary>
        /// 隐藏检测任务
        /// </summary>
        public Task HideCheckTask { get; set; }
        /// <summary>
        /// 是否跳出
        /// </summary>
        public bool IsBreak = false;
        /// <summary>
        /// 是否重置时间
        /// </summary>
        public bool IsResetTime = false;
        /// <summary>
        /// 自动隐藏时长
        /// </summary>
        public int AutoHideTime { get; set; } = 10;

        public FullKeyboardControl()
        {
            InitializeComponent();

            this.Loaded += FullKeyboardControl_Loaded;
            this.IsVisibleChanged += FullKeyboardControl_IsVisibleChanged;
            this.PreviewMouseUp += FullKeyboardControl_PreviewMouseUp;
        }
        /// <summary>
        /// 按键抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullKeyboardControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsResetTime = true;
        }

        private async void FullKeyboardControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                IsBreak = false;
                HideCheckTask = new Task(async () => {
                    try
                    {
                        DateTime now = DateTime.Now;
                        while (!IsBreak && (DateTime.Now - now).TotalSeconds < AutoHideTime)
                        {
                            await Task.Delay(1000);
                            if (IsResetTime)
                            {
                                now = DateTime.Now;
                                IsResetTime = false;
                            }
                        }
                        if (this.Visibility == Visibility.Visible && !IsBreak)
                        {
                            this.Dispatcher.Invoke(() => {
                                this.Visibility = Visibility.Collapsed;
                            });
                            
                            Console.WriteLine("隐藏");
                        }
                    }
                    catch (Exception ex)
                    { 
                    
                    }
                });
                HideCheckTask.Start();
                await HideCheckTask;
            }
            else
            {
                IsBreak = true;
            }
        }

        /// <summary>
        /// 控件加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullKeyboardControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Model;
        }
        
    }
}
