using Microsoft.Maui.Controls;
using System;
using System.Windows.Input;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class MatriculaPage : ContentPage
    {
        public MatriculaPage()
        {
            InitializeComponent();

            BtnSearchCpf.Clicked += BtnSearchCpf_Clicked;
            DataInicio.DateSelected += DataInicio_DateSelected;
            BtnAnexar.Clicked += BtnAnexar_Clicked;
            BtnRemover.Clicked += BtnRemover_Clicked;
            BtnCancelar.Clicked += BtnCancelar_Clicked;
            BtnGuardar.Clicked += BtnGuardar_Clicked;
        }

        private void BtnSearchCpf_Clicked(object? sender, EventArgs e)
        {
            // invoke SearchByCpfCommand on ViewModel if available
            if (BindingContext != null)
            {
                var prop = BindingContext.GetType().GetProperty("SearchByCpfCommand") ?? BindingContext.GetType().GetProperty("SearchByCpfAsyncCommand");
                if (prop != null && prop.GetValue(BindingContext) is ICommand cmd && cmd.CanExecute(CpfSearch.Text))
                {
                    cmd.Execute(CpfSearch.Text);
                }
            }
        }

        private void DataInicio_DateSelected(object? sender, DateChangedEventArgs e)
        {
            // Try to call ViewModel method to recalculate end date
            if (BindingContext != null)
            {
                var method = BindingContext.GetType().GetMethod("RecalculateDataTermino");
                method?.Invoke(BindingContext, new object[] { DataInicio.Date });
            }
        }

        private void BtnAnexar_Clicked(object? sender, EventArgs e)
        {
            var prop = BindingContext?.GetType().GetProperty("AttachLaudoCommand") ?? BindingContext?.GetType().GetProperty("AttachLaudoAsyncCommand");
            if (prop != null && prop.GetValue(BindingContext) is ICommand cmd && cmd.CanExecute(null)) cmd.Execute(null);
        }

        private void BtnRemover_Clicked(object? sender, EventArgs e)
        {
            var prop = BindingContext?.GetType().GetProperty("RemoveLaudoCommand") ?? BindingContext?.GetType().GetProperty("RemoveLaudoAsyncCommand");
            if (prop != null && prop.GetValue(BindingContext) is ICommand cmd && cmd.CanExecute(null)) cmd.Execute(null);
        }

        private void BtnCancelar_Clicked(object? sender, EventArgs e)
        {
            _ = Shell.Current.GoToAsync("..", false);
        }

        private void BtnGuardar_Clicked(object? sender, EventArgs e)
        {
            var prop = BindingContext?.GetType().GetProperty("SaveCommand") ?? BindingContext?.GetType().GetProperty("SaveAsyncCommand");
            if (prop != null && prop.GetValue(BindingContext) is ICommand cmd && cmd.CanExecute(null)) cmd.Execute(null);
        }
    }
}
