using FluentResults;
using LottoNumberRandomizer.Infrastructure.Services;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Queries;
using SimpleCqrs;

namespace LottoNumberRandomizer.ApplicationLayer.Queries;

public class GetRandomNumbersQueryHandler(ILottoNumberService _lottoNumberService) : IAsyncQueryHandler<GetRandomNumbersQuery, Result<IEnumerable<RandomNumbersDto>>>
{
    public async Task<Result<IEnumerable<RandomNumbersDto>>> HandleAsync(GetRandomNumbersQuery query, CancellationToken cancellationToken = default)
        => await _lottoNumberService.GetRandomNumbers(query);
}
