using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dispensing.ExDependencies
{
    public class ListBoxExDependency : DependencyObject
    {
        /// <summary>
        /// 扩展标题属性
        /// </summary>
        public static readonly DependencyProperty ListBoxExTitleProperty = DependencyProperty.RegisterAttached("ListBoxExTitle", typeof(string), typeof(ListBoxExDependency));
        public static string GetListBoxExTitle(DependencyObject dependency)
        {
            return (string)dependency.GetValue(ListBoxExTitleProperty);
        }
        public static void SetListBoxExTitle(DependencyObject dependency, string title)
        {
            dependency.SetValue(ListBoxExTitleProperty, title);
        }


        public static readonly DependencyProperty LostFocusRemoveSelectedProperty = DependencyProperty.RegisterAttached("LostFocusRemoveSelected", typeof(bool), typeof(ListBoxExDependency), new PropertyMetadata(LostFocusRemoveSelectedInit));
        public static void SetLostFocusRemoveSelected(DependencyObject dependency, bool value)
        {
            dependency.SetValue(LostFocusRemoveSelectedProperty, value);
        }
        public static bool GetLostFocusRemoveSelected(DependencyObject dependency)
        {
            return (bool)dependency.GetValue(LostFocusRemoveSelectedProperty);
        }
        /// <summary>
        /// 所有添加的listbox
        /// </summary>
        public static List<ListBox> listBoxes = new List<ListBox>();
        private static void LostFocusRemoveSelectedInit(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ListBox box = d as ListBox;
                box.GotFocus += Box_GotFocus;
                listBoxes.Add(box);
            }
            else
            {
                ListBox box = d as ListBox;
                box.GotFocus += Box_GotFocus;
                if (listBoxes.Contains(box))
                    listBoxes.Remove(box);
            }
        }
        /// <summary>
        /// 获取焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Box_GotFocus(object sender, RoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            foreach (var item in listBoxes)
            {
                if(listBox == item)
                {
                    continue;
                }
                item.SelectedItem = null;
            }
        }

      
    }
}
