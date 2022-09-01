using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace dispensing.ExDependencies
{
    public class RadioButtonExDependency : DependencyObject
    {
        /// <summary>
        /// 是否是radio group组
        /// </summary>
        public static readonly DependencyProperty RadioButtonIsGroupProperty = DependencyProperty.RegisterAttached("RadioButtonIsGroup", typeof(bool), typeof(RadioButtonExDependency), new PropertyMetadata(RadioGroupInitCall));
        private static List<RadioButton> FindChildren(DependencyObject dependency)
        {
            List<RadioButton> radios = new List<RadioButton>();
            var childcount = VisualTreeHelper.GetChildrenCount(dependency);
            if(dependency is null || childcount == 0)return radios;

            for (int i = 0; i < childcount; i++)
            {
                var item = VisualTreeHelper.GetChild(dependency, i);
                if(item is RadioButton)
                {
                    radios.Add((RadioButton)item);
                }
                else
                {
                    radios.AddRange(FindChildren(item));
                }
            }
            return radios;
        }
        /// <summary>
        /// 查找所在的组
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns></returns>
        private static DependencyObject FindGroup(DependencyObject dependency)
        {
            var parent = VisualTreeHelper.GetParent(dependency);
            while (parent != null && (bool)parent.GetValue(RadioButtonIsGroupProperty) != true)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }
        private static async void RadioGroupInitCall(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            await Task.Delay(1000);
            //如果设置
            if ((bool)e.NewValue)
            {
                var children = FindChildren(d);
                foreach (var item in children)
                {
                    item.Checked += (o, oe) => 
                    { 
                        d.SetValue(RadioButtonSelectedItemProperty, item.Content);
                        var group = FindGroup(o as DependencyObject);
                        if (group is null) return;
                        ICommand command = group.GetValue(RadioSelectedChangedCommandProperty) as ICommand;
                        if(command is null == false && command.CanExecute(item.Content))
                        {
                            command.Execute(item.Content);
                        }
                    };
                }
            }
        }

        

        public static bool GetRadioButtonIsGroup(DependencyObject dependency)
        {
            return (bool)dependency.GetValue(RadioButtonIsGroupProperty);
        }
        public static void SetRadioButtonIsGroup(DependencyObject dependency, bool value) 
        {
            dependency.SetValue(RadioButtonIsGroupProperty, value);
        }

        public static readonly DependencyProperty RadioButtonSelectedItemProperty = DependencyProperty.RegisterAttached("RadioButtonSelectedItem", typeof(object), typeof(RadioButtonExDependency));
        public static object GetRadioButtonSelectedItem(DependencyObject dependency)
        {
            return dependency.GetValue(RadioButtonSelectedItemProperty);
        }
        public static void SetRadioButtonSelectedItem(DependencyObject dependency, object value)
        {
            dependency.SetValue(RadioButtonSelectedItemProperty, value);
        }

        public static readonly DependencyProperty RadioSelectedChangedCommandProperty = DependencyProperty.RegisterAttached("RadioSelectedChangedCommand", typeof(ICommand), typeof(RadioButtonExDependency), new PropertyMetadata(null));
        public static ICommand GetRadioSelectedChangedCommand(DependencyObject dependency)
        {
            return (ICommand)dependency.GetValue(RadioSelectedChangedCommandProperty);
        }
        public static void SetRadioSelectedChangedCommand(DependencyObject dependency, ICommand value)
        {
            dependency.SetValue(RadioSelectedChangedCommandProperty, value);
        }


    }
}
