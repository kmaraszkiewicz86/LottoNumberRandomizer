using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Queries;

namespace LottoNumberRandomizer.Infrastructure.Services;

public class LottoNumberService : ILottoNumberService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LottoNumberService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<LottoNumberDto>> GenerateRandomNumbersAsync(GetLottoNumbersQuery query)
    {
        // Simulate async operation with HttpClient
        var httpClient = _httpClientFactory.CreateClient();
        
        // Generate 10 random numbers and count their occurrences
        var random = new Random();
        var numbers = new List<int>();
        
        for (int i = 0; i < 10; i++)
        {
            numbers.Add(random.Next(1, 50));
        }
        
        // Group by number and count occurrences
        var result = numbers
            .GroupBy(n => n)
            .Select(g => new LottoNumberDto 
            { 
                Number = g.Key, 
                Count = g.Count() 
            })
            .OrderBy(x => x.Number)
            .ToList();
        
        await Task.CompletedTask;
        
        return result;
    }
}
