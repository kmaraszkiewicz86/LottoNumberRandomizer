using FluentResults;
using LottoNumberRandomizer.Model.Configuration;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Enums;
using LottoNumberRandomizer.Model.Queries;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LottoNumberRandomizer.Infrastructure.Services;

public class LottoNumberService(HttpClient httpClient, IOptions<LottoApiSettings> options) : ILottoNumberService
{
    private readonly LottoApiSettings _settings = options.Value;

    public async Task<Result<IEnumerable<LottoNumberDto>>> GetLatest(GetLottoNumbersQuery query)
    {
        try
        {
            var (dateFrom, dateTo) = CalculateDateRange(query.DateRange);
            
            var url = $"lotteries/draw-statistics/numbers-frequency?gameType={_settings.GameType}&dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd}";
            
            var response = await httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<IEnumerable<LottoNumberDto>>($"API request failed with status code: {response.StatusCode}");
            }
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<LottoApiResponse>(jsonResponse, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            if (apiResponse?.NumberFrequrency == null)
            {
                return Result.Fail<IEnumerable<LottoNumberDto>>("Invalid API response format");
            }
            
            var result = apiResponse.NumberFrequrency
                .Select(nf => new LottoNumberDto 
                { 
                    Number = nf.Number, 
                    NumberOfOccurrences = nf.NumberOfOccurrences,
                    PercentOfOccurrences = nf.PercentOfOccurrences
                })
                .OrderByDescending(x => x.PercentOfOccurrences)
                .ToList();
            
            return Result.Ok<IEnumerable<LottoNumberDto>>(result);
        }
        catch (HttpRequestException ex)
        {
            return Result.Fail<IEnumerable<LottoNumberDto>>($"Network error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return Result.Fail<IEnumerable<LottoNumberDto>>($"JSON parsing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<LottoNumberDto>>($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<RandomNumbersDto>>> GetRandomNumbers(GetRandomNumbersQuery query)
    {
        try
        {
            // Pobierz statystyki czêstotliwoœci liczb
            var latestResult = await GetLatest(new GetLottoNumbersQuery 
            {
                DateRange = query.DateRange 
            });
            
            if (latestResult.IsFailed)
            {
                return Result.Fail<IEnumerable<RandomNumbersDto>>(latestResult.Errors.First().Message);
            }
            
            var numbersFrequency = latestResult.Value.ToList();
            
            // Przygotuj pule liczb wa¿one procentowo
            var weightedNumbers = new List<int>();
            foreach (var number in numbersFrequency)
            {
                // Im wy¿szy procent, tym wiêcej razy dodajemy liczbê do puli
                int weight = (int)Math.Ceiling((decimal)number.PercentOfOccurrences * 10);
                for (int i = 0; i < weight; i++)
                {
                    weightedNumbers.Add(number.Number);
                }
            }
            
            var random = new Random();
            var results = new List<RandomNumbersDto>();
            
            // Generuj okreœlon¹ liczbê losowañ (zapewne query.TicketCount)
            for (int i = 0; i < query.TicketCount; i++)
            {
                var selectedNumbers = new HashSet<int>();
                
                // Wybierz 6 unikalnych liczb (standardowe lotto)
                while (selectedNumbers.Count < 6)
                {
                    var randomIndex = random.Next(weightedNumbers.Count);
                    selectedNumbers.Add(weightedNumbers[randomIndex]);
                }
                
                results.Add(new RandomNumbersDto 
                { 
                    Numbers = selectedNumbers.OrderBy(x => x).ToArray() 
                });
            }
            
            return Result.Ok<IEnumerable<RandomNumbersDto>>(results);
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RandomNumbersDto>>($"Error generating random numbers: {ex.Message}");
        }
    }

    private (DateTime dateFrom, DateTime dateTo) CalculateDateRange(LottoDateRange dateRange)
    {
        var dateTo = DateTime.Now;
        var dateFrom = dateRange switch
        {
            LottoDateRange.OneMonth => dateTo.AddMonths(-1),
            LottoDateRange.TwoMonths => dateTo.AddMonths(-2),
            LottoDateRange.SixMonths => dateTo.AddMonths(-6),
            LottoDateRange.OneYear => dateTo.AddYears(-1),
            _ => dateTo.AddMonths(-1)
        };
        
        return (dateFrom, dateTo);
    }
}
