using System;
using System.IO;
using System.Globalization;
using AcademiaDoZe.Application.DTOs;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class FotoToImageSourceConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var bytes = value switch
            {
                byte[] content => content,
                ArquivoDto { Conteudo: { Length: > 0 } content } => content,
                _ => null
            };

            if (bytes is not { Length: > 0 })
                return null;

            try
            {
                return ImageSource.FromStream(() => new MemoryStream(bytes, writable: false));
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }
}
