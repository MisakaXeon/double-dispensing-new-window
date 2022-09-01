using dispensing.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace dispensing.Adorners
{
    public class WindowDialogAdorner : Adorner
    {
        /// <summary>
        /// 主UI
        /// </summary>
        public UIElement UIElement { get; set; }
        /// <summary>
        /// 遮罩
        /// </summary>
        public Border Mask { get; set; } = new Border() {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Opacity = 0.6,
        };
        /// <summary>
        /// 遮罩父级
        /// </summary>
        public Grid Parent = new Grid();

        public WindowDialogAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.UIElement = adornedElement;

            Parent.Children.Add(this.Mask);
            this.AddVisualChild(this.Parent);
            Mask.Background = null;
        }

        protected override Visual GetVisualChild(int index)
        {
            return Parent;
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="control"></param>
        public void ShowDialog(CustomDialogControl control)
        {
            Parent.Children.Add(control);
            Mask.Background = control.MaskBrush;
            Mask.Opacity = control.MaskOpacity;
        }
        /// <summary>
        /// 添加键盘显示
        /// </summary>
        public void AddKeyboard()
        {
            FullKeyboardControl fullKeyboard = new FullKeyboardControl();
            fullKeyboard.HorizontalAlignment = HorizontalAlignment.Stretch;
            fullKeyboard.VerticalAlignment = VerticalAlignment.Bottom;
            Parent.Children.Add(fullKeyboard);
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void CloseDialog()
        {
            this.Mask.Child = null;
            this.Mask.Background = null;
        }
        /// <summary>
        /// 裁剪尺寸
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Parent.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }
        /// <summary>
        /// 子元素数量
        /// </summary>
        protected override int VisualChildrenCount => 1;
    }
}
