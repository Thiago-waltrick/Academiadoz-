using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class LogradouroListPage : ContentPage
    {
        private readonly LogradouroListViewModel _viewModel;

        public LogradouroListPage(LogradouroListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadLogradourosCommand.ExecuteAsync(null);
        }

        async void OnEditButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.BindingContext is LogradouroDto logradouro)
                {
                    await Shell.Current.GoToAsync($"logradouro?Id={logradouro.Id}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.BindingContext is LogradouroDto item)
                {
                    var confirm = await DisplayAlert("Confirmar", "Deseja remover este logradouro?", "Sim", "Não");
                    if (!confirm) return;

                    if (BindingContext != null)
                    {
                        var vm = BindingContext;
                        var prop = vm.GetType().GetProperty("DeleteLogradouroCommand") ?? vm.GetType().GetProperty("DeleteLogradouroAsyncCommand");
                        if (prop != null)
                        {
                            var cmd = prop.GetValue(vm) as System.Windows.Input.ICommand;
                            if (cmd != null && cmd.CanExecute(item))
                            {
                                cmd.Execute(item);
                                return;
                            }
                        }
                    }

                    await DisplayAlert("Erro", "Comando de remoção não encontrado.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}
