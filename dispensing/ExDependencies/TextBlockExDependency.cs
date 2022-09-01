using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace dispensing.ExDependencies
{
    public class TextBlockExDependency : DependencyObject
    {

        public static readonly DependencyProperty TimerFormatProperty = DependencyProperty.RegisterAttached("TimerFormat", typeof(string), typeof(TextBlockExDependency), new PropertyMetadata(TimerFormatInit));

        private static void TimerFormatInit(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)d;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            textBlock.Text = DateTime.Now.ToString(e.NewValue.ToString());
            timer.Tick += new EventHandler((o, oe) => {
                textBlock.Text = DateTime.Now.ToString(e.NewValue.ToString());
            });
            timer.Start();
        }
        public static string GetTimerFormat(DependencyObject dependency)
        {
            return dependency.GetValue(TimerFormatProperty) as string;
        }
        public static void SetTimerFormat(DependencyObject dependency, string format)
        {
            dependency.SetValue(TimerFormatProperty, format);
        }
    }
}
