using FluentResults;
using LottoNumberRandomizer.Infrastructure.Services;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Queries;
using SimpleCqrs;

namespace LottoNumberRandomizer.ApplicationLayer.Queries;

public class GetLottoNumbersQueryHandler(ILottoNumberService _lottoNumberService) : IAsyncQueryHandler<GetLottoNumbersQuery, Result<IEnumerable<LottoNumberDto>>>
{
    public async Task<Result<IEnumerable<LottoNumberDto>>> HandleAsync(GetLottoNumbersQuery query, CancellationToken cancellationToken = default)
        => await _lottoNumberService.GetLatest(query);
}
