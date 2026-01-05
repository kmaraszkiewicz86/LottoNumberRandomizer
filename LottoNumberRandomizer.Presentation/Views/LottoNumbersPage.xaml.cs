using LottoNumberRandomizer.Presentation.ViewModels;

namespace LottoNumberRandomizer.Presentation.Views;

public partial class LottoNumbersPage : ContentPage
{
    public LottoNumbersPage(LottoNumbersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
