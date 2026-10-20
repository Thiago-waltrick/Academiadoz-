using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class EnumDisplayConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
                return string.Empty;

            var enumType = value.GetType();
            var enumName = value.ToString() ?? string.Empty;
            if (!enumType.IsEnum)
                return enumName;

            try
            {
                var displayName = enumType.GetMember(enumName)
                    .FirstOrDefault()?
                    .GetCustomAttribute<DisplayAttribute>()?
                    .GetName();

                return string.IsNullOrWhiteSpace(displayName) ? enumName : displayName;
            }
            catch
            {
                return enumName;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }
}
