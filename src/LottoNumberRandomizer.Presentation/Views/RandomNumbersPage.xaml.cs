using LottoNumberRandomizer.Presentation.ViewModels;

namespace LottoNumberRandomizer.Presentation.Views;

public partial class RandomNumbersPage : ContentPage
{
	public RandomNumbersPage(RandomNumbersPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}