using Guauseo.ViewModels;
using Guauseo.Models;

namespace Guauseo.Views;

public partial class SolicitarPaseoView : ContentPage
{
	public SolicitarPaseoView(decimal due�oCodigo)
	{
		InitializeComponent();
        BindingContext = new SolicitarPaseoViewModel(due�oCodigo, Navigation);

    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = BindingContext as SolicitarPaseoViewModel;
        if (viewModel == null) return;

        var selectedItems = e.CurrentSelection.Cast<MascotaModel>().ToList();
        viewModel.UpdateSelectedMascotas(selectedItems);
    }
}