using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dispensing.CustomControls
{
    public class CustomGridPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement element in InternalChildren)
            {
                element.Measure(size);
            }
            return new Size();
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            var width = finalSize.Width / InternalChildren.Count;
            for(int i = 0; i< InternalChildren.Count; i ++)
            {
                var element = InternalChildren[i];
                double x = i * width;
                double y = 0;
                element.Arrange(new Rect(new Point(x, y), new Size(width, element.DesiredSize.Height)));
            }
            return finalSize;
        }
    }
}
