using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LottoNumberRandomizer.ApplicationLayer.Queries;
using LottoNumberRandomizer.Model.DTOs;
using SimpleCqrs;
using System.Collections.ObjectModel;

namespace LottoNumberRandomizer.Presentation.ViewModels;

public partial class LottoNumbersViewModel(ISimpleMediator _simpleMediator) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LottoNumberDto> lottoNumbers = new();

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand]
    private async Task GenerateNumbersAsync()
    {
        IsLoading = true;
        try
        {
            var query = new GetLottoNumbersQuery();
            var result = await _simpleMediator.GetQueryAsync(query);
            
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
