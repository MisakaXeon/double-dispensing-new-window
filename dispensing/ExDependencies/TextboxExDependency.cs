using dispensing.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace dispensing.ExDependencies
{
    public class TextboxExDependency : DependencyObject
    {
        /// <summary>
        /// 全键盘控件 需要每个主窗口自行来 进行设置这个变量 窗口创建初始化的时候来 初始化这个变量
        /// </summary>
        public static FullKeyboardControl FullKeyboardControl = null;


        public static readonly DependencyProperty TextWaterProperty = DependencyProperty.RegisterAttached("TextWater", typeof(string), typeof(TextboxExDependency), new PropertyMetadata("请输入..."));
        public static string GetTextWater(DependencyObject dependency)
        {
            return (string)dependency.GetValue(TextWaterProperty);
        }
        public static void SetTextWater(DependencyObject dependency, string value)
        {
            dependency.SetValue(TextWaterProperty, value);
        }

        public static readonly DependencyProperty WaterTextMarginProperty = DependencyProperty.RegisterAttached("WaterTextMargin", typeof(Thickness), typeof(TextboxExDependency), new PropertyMetadata(new Thickness(0)));
        public static Thickness GetWaterTextMargin(DependencyObject dependency)
        {
            return (Thickness)dependency.GetValue(WaterTextMarginProperty);
        }
        public static void SetWaterTextMargin(DependencyObject dependency, Thickness thickness)
        {
            dependency.SetValue(WaterTextMarginProperty, thickness);
        }

        public static readonly DependencyProperty WaterTextFontSizeProperty = DependencyProperty.RegisterAttached("WaterTextFontSize", typeof(double), typeof(TextboxExDependency), new PropertyMetadata(15.0d));
        public static double GetWaterTextFontSize(DependencyObject dependency)
        {
            return (double)dependency.GetValue(WaterTextFontSizeProperty);
        }
        public static void SetWaterTextFontSize(DependencyObject dependency, double thickness)
        {
            dependency.SetValue(WaterTextFontSizeProperty, thickness);
        }

        public static readonly DependencyProperty WaterTextFontWeightProperty = DependencyProperty.RegisterAttached("WaterTextFontWeight", typeof(FontWeight), typeof(TextboxExDependency), new PropertyMetadata(new FontWeight()));
        public static FontWeight GetWaterTextFontWeight(DependencyObject dependency)
        {
            return (FontWeight)dependency.GetValue(WaterTextFontWeightProperty);
        }
        public static void SetWaterTextFontWeight(DependencyObject dependency, FontWeight fontWeight)
        {
            dependency.SetValue(WaterTextFontWeightProperty, fontWeight);
        }

        public static readonly DependencyProperty WaterTextColorProperty = DependencyProperty.RegisterAttached("WaterTextColor", typeof(Brush), typeof(TextboxExDependency), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public static Brush GetWaterTextColor(DependencyObject dependency)
        {
            return (Brush)dependency.GetValue(WaterTextColorProperty);
        }
        public static void SetWaterTextColor(DependencyObject dependency, Brush color)
        {
            dependency.SetValue(WaterTextColorProperty, color);
        }


        public static readonly DependencyProperty RadiusProperty = DependencyProperty.RegisterAttached("Radius", typeof(CornerRadius), typeof(TextboxExDependency), new PropertyMetadata(new CornerRadius(3)));
        public static CornerRadius GetRadius(DependencyObject dependency)
        {
            return (CornerRadius)dependency.GetValue(RadiusProperty);
        }
        public static void SetRadius(DependencyObject dependency, CornerRadius color)
        {
            dependency.SetValue(RadiusProperty, color);
        }

        public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.RegisterAttached("ContentMargin", typeof(Thickness), typeof(TextboxExDependency));
        public static Thickness GetContentMargin(DependencyObject dependency)
        {
            return (Thickness)dependency.GetValue(RadiusProperty);
        }
        public static void SetContentMargin(DependencyObject dependency, Thickness color)
        {
            dependency.SetValue(RadiusProperty, color);
        }

        public static readonly DependencyProperty IsBindKeyboardProperty = DependencyProperty.RegisterAttached("IsBindKeyboard", typeof(bool), typeof(TextboxExDependency), new PropertyMetadata(IsLostFocusCloseKeyboardInit));
        public static void SetIsBindKeyboard(DependencyObject dependency, bool value)
        {
            dependency?.SetValue(IsBindKeyboardProperty, value);
        }
        public static bool GetIsBindKeyboard(DependencyObject dependency)
        {
            return (bool)dependency.GetValue(IsBindKeyboardProperty);
        }
        /// <summary>
        /// 键盘弹出的方位
        /// </summary>
        public static readonly DependencyProperty KeyBorderVerticalAlignmentProperty = DependencyProperty.RegisterAttached("KeyBorderVerticalAlignment",
            typeof(VerticalAlignment), typeof(TextboxExDependency), new PropertyMetadata(VerticalAlignment.Bottom));
        public static VerticalAlignment GetKeyBorderVerticalAlignment(DependencyObject dependency)
        {
            return (VerticalAlignment)dependency.GetValue(KeyBorderVerticalAlignmentProperty);
        }
        public static void SetKeyBorderVerticalAlignment(DependencyObject dependency, VerticalAlignment value)
        {
            dependency.SetValue(KeyBorderVerticalAlignmentProperty, value);
        }
        private static void IsLostFocusCloseKeyboardInit(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox box = d as TextBox;
            if((bool)e.NewValue == true)
            {
                box.GotFocus += Box_GotFocus;
                box.LostFocus += Box_LostFocus;
            }
            else
            {
                box.GotFocus -= Box_GotFocus;
                box.LostFocus -= Box_LostFocus;
            }
        }

        private static void Box_LostFocus(object sender, RoutedEventArgs e)
        {
            if(FullKeyboardControl is null == false)
            {
                FullKeyboardControl.Visibility = Visibility.Collapsed;
            }
        }

        private static void Box_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FullKeyboardControl is null == false)
            {
                FullKeyboardControl.Visibility = Visibility.Visible;
                FullKeyboardControl.VerticalAlignment = (VerticalAlignment)(sender as DependencyObject).GetValue(KeyBorderVerticalAlignmentProperty);
            }
        }
    }
}
