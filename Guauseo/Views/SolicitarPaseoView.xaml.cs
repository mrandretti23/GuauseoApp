using Guauseo.ViewModels;
using Guauseo.Models;

namespace Guauseo.Views;

public partial class SolicitarPaseoView : ContentPage
{
	public SolicitarPaseoView(decimal dueñoCodigo)
	{
		InitializeComponent();
        BindingContext = new SolicitarPaseoViewModel(dueñoCodigo, Navigation);

    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = BindingContext as SolicitarPaseoViewModel;
        if (viewModel == null) return;

        var selectedItems = e.CurrentSelection.Cast<MascotaModel>().ToList();
        viewModel.UpdateSelectedMascotas(selectedItems);
    }
}