using LottoNumberRandomizer.Infrastructure.Services;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Queries;
using SimpleCqrs;

namespace LottoNumberRandomizer.ApplicationLayer.Queries;

public class GetLottoNumbersQueryHandler(ILottoNumberService _lottoNumberService) : IAsyncQueryHandler<GetLottoNumbersQuery, IEnumerable<LottoNumberDto>>
{
    public async Task<IEnumerable<LottoNumberDto>> HandleAsync(GetLottoNumbersQuery query, CancellationToken cancellationToken = default)
    {
        return await _lottoNumberService.GetLatest(query);
    }
}
