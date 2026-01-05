using LottoNumberRandomizer.Infrastructure.Services;
using LottoNumberRandomizer.Model.DTOs;
using SimpleCqrs;

namespace LottoNumberRandomizer.ApplicationLayer.Queries;

public class GetLottoNumbersQueryHandler : IAsyncQueryHandler<GetLottoNumbersQuery, IEnumerable<LottoNumberDto>>
{
    private readonly ILottoNumberService _lottoNumberService;

    public GetLottoNumbersQueryHandler(ILottoNumberService lottoNumberService)
    {
        _lottoNumberService = lottoNumberService;
    }

    public async Task<IEnumerable<LottoNumberDto>> HandleAsync(GetLottoNumbersQuery query, CancellationToken cancellationToken = default)
    {
        return await _lottoNumberService.GenerateRandomNumbersAsync();
    }
}
