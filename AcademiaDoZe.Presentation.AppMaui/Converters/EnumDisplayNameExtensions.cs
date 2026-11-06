using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AcademiaDoZe.Presentation.AppMaui.Converters;

public static class EnumDisplayNameExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var name = value.ToString();
        return value.GetType().GetField(name)?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? name;
    }
}
