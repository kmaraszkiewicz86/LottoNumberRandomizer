using FluentResults;
using LottoNumberRandomizer.Model.Configuration;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Enums;
using LottoNumberRandomizer.Model.Queries;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LottoNumberRandomizer.Infrastructure.Services;

public class LottoNumberService(HttpClient httpClient, IOptions<LottoApiSettings> options, IMemoryCache memoryCache) : ILottoNumberService
{
    private readonly LottoApiSettings _settings = options.Value;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<Result<IEnumerable<LottoNumberDto>>> GetLatest(GetLottoNumbersQuery query)
    {
        try
        {
            var (dateFrom, dateTo) = CalculateDateRange(query.DateRange);
            
            // Generate cache key based on date range and game type
            var cacheKey = $"LottoNumbers_{_settings.GameType}_{query.DateRange}_{dateFrom:yyyy-MM-dd}_{dateTo:yyyy-MM-dd}";
            
            // Try to get data from cache
            if (memoryCache.TryGetValue<IEnumerable<LottoNumberDto>>(cacheKey, out var cachedResult) && cachedResult != null)
            {
                return Result.Ok(cachedResult);
            }
            
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
            
            // Store result in cache with 1-day expiration
            memoryCache.Set(cacheKey, result, CacheDuration);
            
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
            // Get number frequency statistics
            var latestResult = await GetLatest(new GetLottoNumbersQuery 
            {
                DateRange = query.DateRange 
            });
            
            if (latestResult.IsFailed)
            {
                return Result.Fail<IEnumerable<RandomNumbersDto>>(latestResult.Errors.First().Message);
            }
            
            var numbersFrequency = latestResult.Value.ToList();
            
            // Prepare a weighted pool of numbers based on their percentage
            var weightedNumbers = new List<int>();
            foreach (var number in numbersFrequency)
            {
                // The higher the percentage, the more times we add the number to the pool
                int weight = (int)Math.Ceiling((decimal)number.PercentOfOccurrences * 10);
                for (int i = 0; i < weight; i++)
                {
                    weightedNumbers.Add(number.Number);
                }
            }
            
            var random = new Random();
            var results = new List<RandomNumbersDto>();
            
            // Generate the specified number of random draws (query.TicketCount)
            for (int i = 0; i < query.TicketCount; i++)
            {
                var selectedNumbers = new HashSet<int>();
                
                // Select 6 unique numbers (standard lotto)
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
