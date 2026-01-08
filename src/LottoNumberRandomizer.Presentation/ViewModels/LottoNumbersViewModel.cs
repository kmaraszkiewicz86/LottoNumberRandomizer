using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Enums;
using LottoNumberRandomizer.Model.Queries;
using LottoNumberRandomizer.Presentation.Models;
using LottoNumberRandomizer.Presentation.Resources.Localization;
using SimpleCqrs;
using System.Collections.ObjectModel;

namespace LottoNumberRandomizer.Presentation.ViewModels;

public partial class LottoNumbersViewModel(ISimpleMediator _simpleMediator) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LottoNumberDto> lottoNumbers = new();

    public bool IsLoading
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            OnPropertyChanged(nameof(IsLoaded));
        }
    }

    public bool IsLoaded => !IsLoading;

    [ObservableProperty]
    private int lastDrawsCount = 10;

    public LottoDateRangeOption SelectedDateRange
    {
        get => field ?? AvailableDateRanges.First(x => x.Value == LottoDateRange.TwoMonths);
        set => SetProperty(ref field, value);
    }

    public string ErrorMessage
    {
        get => field;
        set
        {
            SetProperty(ref field, value);
            OnPropertyChanged(nameof(HasErrorMessage));
        }
    } = string.Empty;

    public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

    public IReadOnlyList<LottoDateRangeOption> AvailableDateRanges { get; } =
    [
        new(LottoDateRange.OneMonth, AppResources.OneMonth),
        new(LottoDateRange.TwoMonths, AppResources.TwoMonths),
        new(LottoDateRange.SixMonths, AppResources.SixMonths),
        new(LottoDateRange.OneYear, AppResources.OneYear)
    ];

    [RelayCommand]
    private async Task GenerateNumbersAsync()
    {
        ErrorMessage = string.Empty;

        if (LastDrawsCount <= 0 || LastDrawsCount > 20)
        {
            ErrorMessage = AppResources.LastDrawsCountValidationError;
            return;
        }

        if (SelectedDateRange is null)
        {
            ErrorMessage = AppResources.DateRangeValidationError;
            return;
        }

        IsLoading = true;
        try
        {
            var query = new GetLottoNumbersQuery
            {
                DateRange = SelectedDateRange.Value
            };

            var result = await _simpleMediator.GetQueryAsync(query);

            if (result.IsFailed)
            {
                ErrorMessage = AppResources.ApiErrorMessage;
                return;
            }

            LottoNumbers.Clear();
            foreach (var number in result.Value)
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
