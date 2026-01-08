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
            
            if (apiResponse?.NumbersFrequency == null)
            {
                return Result.Fail<IEnumerable<LottoNumberDto>>("Invalid API response format");
            }
            
            var result = apiResponse.NumbersFrequency
                .Select(nf => new LottoNumberDto 
                { 
                    Number = nf.Number, 
                    Count = nf.Frequency 
                })
                .OrderBy(x => x.Number)
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
