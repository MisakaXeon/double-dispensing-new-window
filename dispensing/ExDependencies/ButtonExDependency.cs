using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsApiLib.API;

namespace dispensing.ExDependencies
{
    public class ButtonExDependency : DependencyObject
    {
        /// <summary>
        /// 圆角
        /// </summary>
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.RegisterAttached("Radius", typeof(CornerRadius), typeof(ButtonExDependency), new PropertyMetadata(new CornerRadius(3)));
        public static CornerRadius GetRadius(DependencyObject dependency)
        {
            return (CornerRadius)dependency.GetValue(RadiusProperty);    
        }
        public static void SetRadius(DependencyObject dependency, CornerRadius radius)
        {
            dependency.SetValue(RadiusProperty, radius);
        }
        /// <summary>
        /// 是否按钮组
        /// </summary>
        public static readonly DependencyProperty IsKeyGroupProperty = DependencyProperty.RegisterAttached("IsKeyGroup", typeof(bool), typeof(ButtonExDependency));
        public static bool GetIsKeyGroup(DependencyObject dependency)
        {
            return (bool)dependency.GetValue(IsKeyGroupProperty);
        }
        public static void SetIsKeyGroup(DependencyObject dependency, bool value)
        {
            dependency.SetValue(IsKeyGroupProperty, value);
        }

        public static readonly DependencyProperty IsLowerProperty = DependencyProperty.RegisterAttached("IsLower", typeof(bool), typeof(ButtonExDependency), new PropertyMetadata(false));
        public static bool GetIsLower(DependencyObject dependency)
        {
            return (bool)dependency.GetValue(IsLowerProperty);
        }
        public static void SetIsLower(DependencyObject dependency, bool value)
        {
            dependency.SetValue(IsLowerProperty, value);
        }
        /// <summary>
        /// 是否是被包含的子按键
        /// </summary>
        public static readonly DependencyProperty IsChildKeyProperty = DependencyProperty.RegisterAttached("IsChildKey", typeof(bool), typeof(ButtonExDependency), new PropertyMetadata(false));
        public static bool GetIsChildKey(DependencyObject dependency)
        {
            return (bool)dependency.GetValue(IsChildKeyProperty);
        }
        public static void SetIsChildKey(DependencyObject dependency, bool value)
        {
            dependency.SetValue(IsChildKeyProperty, value);
        }
        public static DependencyObject FindGroup(DependencyObject dependency)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dependency);
            while (parent != null && (parent.GetValue(IsKeyGroupProperty) == null || (bool)parent.GetValue(IsKeyGroupProperty) == false))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }
        /// <summary>
        /// 查找所有的可以转换的子元素
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static List<Button> FindChildKeys(DependencyObject parent)
        {
            List<Button> keys = new List<Button>();
            if(parent is null)return keys;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if(count == 0)return keys;

            for (int i = 0; i < count; i++)
            {
                DependencyObject item = VisualTreeHelper.GetChild(parent, i);
                if((bool)item.GetValue(IsChildKeyProperty) == true && item is Button)
                {
                    keys.Add((Button)item);
                }
                else if(item is Button == false)
                {
                    keys.AddRange(FindChildKeys(item));
                }
            }
            return keys;
        }
        /// <summary>
        /// 变成小写
        /// </summary>
        public static readonly DependencyProperty ToLowerProperty = DependencyProperty.RegisterAttached("ToLower", typeof(bool), typeof(ButtonExDependency), new PropertyMetadata(new PropertyChangedCallback(ToLowerInitEvent)));

        private static async void ToLowerInitEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            
            if ((bool)e.NewValue == true)
            {
                Button button = d as Button;
                button.Click += (o, oe) =>
                {
                    var group = FindGroup(o as DependencyObject);
                    if (group == null) return;
                    if ((bool)group.GetValue(IsLowerProperty) == true)
                    {
                        var btns = FindChildKeys(group);
                        foreach (var item in btns)
                        {
                            item.Content = item.Content.ToString().ToUpper();
                        }
                        WindowsAPI.SetCapital(true);
                        group.SetValue(IsLowerProperty, false);
                        return;
                    }
                    else
                    {
                        var btns = FindChildKeys(group);
                        foreach (var item in btns)
                        {
                            item.Content = item.Content.ToString().ToLower();
                        }
                        WindowsAPI.SetCapital(false);
                        group.SetValue(IsLowerProperty, true);
                    }
                };
            }

            await Task.Delay(500);
            var group = FindGroup(btn);
            if (CapsLockStatus)
            {
                group.SetValue(IsLowerProperty, false);
            }
            else
            {
                group.SetValue(IsLowerProperty, true);
            }
        }
        public static bool GetToLower(DependencyObject dependency)
        {
            return (bool)dependency.GetValue(ToLowerProperty);
        }
        public static void SetToLower(DependencyObject dependency, bool value)
        {
            dependency.SetValue(ToLowerProperty, value);
        }
        ///// <summary>
        ///// 变成大写
        ///// </summary>
        //public static readonly DependencyProperty ToUpperProperty = DependencyProperty.RegisterAttached("ToUpper", typeof(bool), typeof(ButtonExDependency), new PropertyMetadata(new PropertyChangedCallback(ToUpperInitEvent)));

        //private static void ToUpperInitEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if ((bool)e.NewValue == true)
        //    {
        //        Button button = d as Button;
        //        button.Click += (o, oe) =>
        //        {
        //            var group = FindGroup(o as DependencyObject);
        //            if (group == null || (bool)group.GetValue(IsLowerProperty) == false)
        //            {
        //                return;
        //            }
        //            var btns = FindChildKeys(group);
        //            foreach (var item in btns)
        //            {
        //                item.Content = item.Content.ToString().ToUpper();
        //            }
        //            WindowsAPI.SetCapital(true);
        //            group.SetValue(IsLowerProperty, false);
        //        };
        //    }
        //}

        //public static bool GetToUpper(DependencyObject dependency)
        //{
        //    return (bool)dependency.GetValue(ToUpperProperty);
        //}
        //public static void SetToUpper(DependencyObject dependency, bool value)
        //{
        //    dependency.SetValue(ToUpperProperty, value);
        //}
        public static bool CapsLockStatus
        {
            get
            {
                byte[] bs = new byte[256];
                GetKeyboardState(bs);
                return (bs[0x14] == 1);
            }
        }

        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
    }
}
