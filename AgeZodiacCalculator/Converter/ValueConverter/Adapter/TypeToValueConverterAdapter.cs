﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace AgeZodiacCalculator.Converter.ValueConverter.Adapter
{
    public class TypeToValueConverterAdapter<T> : IValueConverter
    {
        private readonly TypeConverter _typeConverter;

        public TypeToValueConverterAdapter()
        {
            _typeConverter = TypeDescriptor.GetConverter(typeof(T));
        }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return _typeConverter.ConvertTo(null, culture, value, targetType) ?? "Unknown";
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack functionality is not implemented for TypeToValueConverterAdapter");
        }
    }
}