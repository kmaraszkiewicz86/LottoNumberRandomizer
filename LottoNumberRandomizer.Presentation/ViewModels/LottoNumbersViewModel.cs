using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LottoNumberRandomizer.ApplicationLayer.Queries;
using LottoNumberRandomizer.Model.DTOs;
using SimpleCqrs;
using System.Collections.ObjectModel;

namespace LottoNumberRandomizer.Presentation.ViewModels;

public partial class LottoNumbersViewModel : ObservableObject
{
    private readonly ISimpleMediator _simpleMediator;

    [ObservableProperty]
    private ObservableCollection<LottoNumberDto> lottoNumbers = new();

    [ObservableProperty]
    private bool isLoading;

    public LottoNumbersViewModel(ISimpleMediator simpleMediator)
    {
        _simpleMediator = simpleMediator;
    }

    [RelayCommand]
    private async Task GenerateNumbersAsync()
    {
        IsLoading = true;
        try
        {
            var query = new GetLottoNumbersQuery();
            var result = await Task.Run(() => _simpleMediator.GetQuery(query));
            
            LottoNumbers.Clear();
            foreach (var number in result)
            {
                LottoNumbers.Add(number);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
