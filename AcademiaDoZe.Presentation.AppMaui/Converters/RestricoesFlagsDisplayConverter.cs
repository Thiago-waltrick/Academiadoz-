using System;
using System.Linq;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    /// <summary>
    /// Converte um enum marcado com [Flags] para uma string com nomes amigáveis
    /// usando o atributo [Display(Name = "...")], separando por vírgulas.
    /// </summary>
    public class RestricoesFlagsDisplayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var enumType = value.GetType();
            if (!enumType.IsEnum) return value.ToString() ?? string.Empty;

            var longVal = System.Convert.ToInt64(value);
            if (longVal == 0) return ((Enum)value).GetDisplayName();

            var names = Enum.GetValues(enumType).Cast<object>()
                .Where(v => (System.Convert.ToInt64(v) & longVal) != 0)
                .Select(v => ((Enum)v).GetDisplayName())
                .Where(s => !string.IsNullOrWhiteSpace(s));

            return string.Join(", ", names);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
