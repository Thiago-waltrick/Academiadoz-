using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Presentation.AppMaui.Converters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public sealed class RestricaoSelectable : ObservableObject
{
    private AppMatriculaRestricoes _value;
    private bool _isSelected;

    public AppMatriculaRestricoes Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
                OnPropertyChanged(nameof(Nome));
        }
    }

    public string Nome => Value.GetDisplayName();

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
