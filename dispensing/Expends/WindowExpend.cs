using dispensing.Adorners;
using dispensing.CustomControls;
using dispensing.ExDependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace dispensing.Expends
{
    public static class WindowExpend
    {
        /// <summary>
        /// 从父级查找对应的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public static T FindParentVisiual<T>(this DependencyObject dependency) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dependency);
            while (parent is T == false && parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return (T)parent;
        }

        /// <summary>
        /// 弹出任意对话框
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="toastDialog"></param>
        public static async Task ShowMessageAsync(this ContentControl mainWindow, CustomDialogControl dialog, bool isContainsKeyboard = false)
        {
            var window = mainWindow.Content as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(window);
            var ad = new WindowDialogAdorner(window);
            layer.Add(ad);
            ad.ShowDialog(dialog);
            if (isContainsKeyboard)
            {
                ad.AddKeyboard();
            }
            await dialog.Wait();
            layer.Remove(ad);
        }


        public static void CloseKeyboard(this FrameworkElement frameworkElement)
        {
            TextboxExDependency.FullKeyboardControl.Visibility = Visibility.Collapsed;
        }

        public static void ShowKeyboard(this FrameworkElement frameworkElement)
        {
            TextboxExDependency.FullKeyboardControl.Visibility = Visibility.Visible;
        }
    }
}
