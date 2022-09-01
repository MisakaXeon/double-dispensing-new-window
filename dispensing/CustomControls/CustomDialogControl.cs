using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace dispensing.CustomControls
{
    public class CustomDialogControl : UserControl
    {
        /// <summary>
        /// 是否已经关闭
        /// </summary>
        public bool IsClosed { get; private set; } = false;
        /// <summary>
        /// 事件 信号
        /// </summary>
        public ManualResetEvent AutoReset = new ManualResetEvent(false);
        /// <summary>
        /// 遮罩透明度
        /// </summary>
        public double MaskOpacity { get; set; } = 0.5;
        /// <summary>
        /// 遮罩背景颜色
        /// </summary>
        public Brush MaskBrush { get; set; } = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
        /// <summary>
        /// 窗口关闭回调事件
        /// </summary>
        public Action<bool?> CloseAction { get; set; }

        public CustomDialogControl()
        {
            this.Background = null;
            this.CloseAction += a =>
            {
                IsClosed = true;
                AutoReset.Set();
            };
        }
        /// <summary>
        /// 异步请求关闭窗口
        /// </summary>
        /// <returns></returns>
        public void CloseAsync()
        {
            if (IsClosed) return;
            CloseAction?.Invoke(null);
        }
        /// <summary>
        /// 等待关闭
        /// </summary>
        /// <returns></returns>
        public async Task Wait()
        {
            if (IsClosed) return;
            var task = new Task(() => {
                try
                {
                    AutoReset.WaitOne();
                }
                catch (Exception)
                {
                }
            });
            task.Start();
            await task;
        }
    }
}
