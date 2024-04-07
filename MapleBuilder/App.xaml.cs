using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace MapleBuilder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void PART_Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb?.TemplatedParent is not Slider slider) return;

            double changed = e.HorizontalChange / slider.ActualWidth * (slider.Maximum - slider.Minimum);
            double newValue = slider.Value + Math.Round(changed / slider.SmallChange) * slider.SmallChange;
            slider.Value = Math.Clamp(newValue, slider.Minimum, slider.Maximum);
        }
    }

    public class SliderValueToMarginConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {        
            double minimum = (double)values[0];
            double maximum = (double)values[1];
            double value = Math.Clamp((double)values[2], minimum, maximum);
            double sliderWidth = (double)values[3] - 18;
            
            double relativePosition = (value - minimum) / (maximum - minimum);

            double margin = relativePosition * sliderWidth;
            return new Thickness(margin, 0, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}