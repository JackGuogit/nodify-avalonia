﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Nodify
{
    internal class UnscaleTransformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Transform result = new MatrixTransform(((TransformGroup)value).Children[0].Value.Invert());
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    internal class UnscaleDoubleConverter : IMultiValueConverter
    {
        public object Convert(IList<object?> values, Type targetType, object parameter, CultureInfo culture)
        {
            double result = (double)values[0] * (double)values[1];
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
