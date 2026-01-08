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

public partial class RandomNumbersPageViewModel(ISimpleMediator _simpleMediator) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<RandomNumbersDto> randomNumbers = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private int ticketCount = 1;

    [ObservableProperty]
    private int lastDrawsCount = 10;

    [ObservableProperty]
    private LottoDateRangeOption selectedDateRange = new(LottoDateRange.OneMonth, AppResources.OneMonth);

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
    private async Task GenerateRandomNumbersAsync()
    {
        ErrorMessage = string.Empty;

        if (TicketCount <= 0 || TicketCount > 10)
        {
            ErrorMessage = AppResources.TicketCountValidationError;
            return;
        }

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
            var query = new GetRandomNumbersQuery
            {
                TicketCount = TicketCount,
                NumberCount = LastDrawsCount,
                DateRange = SelectedDateRange.Value
            };

            var result = await _simpleMediator.GetQueryAsync(query);

            if (result.IsFailed)
            {
                ErrorMessage = AppResources.ApiErrorMessage;
                return;
            }

            RandomNumbers.Clear();
            foreach (var numbers in result.Value)
            {
                RandomNumbers.Add(numbers);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
