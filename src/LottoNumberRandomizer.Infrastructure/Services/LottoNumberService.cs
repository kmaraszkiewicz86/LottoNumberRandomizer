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

    public async Task<IEnumerable<LottoNumberDto>> GetLatest(GetLottoNumbersQuery query)
    {
        var (dateFrom, dateTo) = CalculateDateRange(query.DateRange);
        
        var url = $"lotteries/draw-statistics/numbers-frequency?gameType={_settings.GameType}&dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd}";
        
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<LottoApiResponse>(jsonResponse, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
        
        var result = apiResponse?.NumbersFrequency?
            .Select(nf => new LottoNumberDto 
            { 
                Number = nf.Number, 
                Count = nf.Frequency 
            })
            .OrderBy(x => x.Number)
            .ToList() ?? new List<LottoNumberDto>();
        
        return result;
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
