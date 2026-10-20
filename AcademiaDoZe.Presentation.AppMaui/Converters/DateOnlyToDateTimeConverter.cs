using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
                return DateTime.Today;

            try
            {
                return value switch
                {
                    DateOnly date => date.ToDateTime(TimeOnly.MinValue),
                    DateTime dateTime => dateTime.Date,
                    string text when DateOnly.TryParse(text, culture, DateTimeStyles.None, out var date) =>
                        date.ToDateTime(TimeOnly.MinValue),
                    _ => Binding.DoNothing
                };
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
                return Nullable.GetUnderlyingType(targetType) is not null ? null! : Binding.DoNothing;

            try
            {
                return value switch
                {
                    DateOnly date => date,
                    DateTime dateTime => DateOnly.FromDateTime(dateTime),
                    string text when DateTime.TryParse(text, culture, DateTimeStyles.None, out var dateTime) =>
                        DateOnly.FromDateTime(dateTime),
                    _ => Binding.DoNothing
                };
            }
            catch
            {
                return Binding.DoNothing;
            }
        }
    }
}
