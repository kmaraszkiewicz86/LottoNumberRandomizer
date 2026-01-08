using FluentResults;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Enums;
using SimpleCqrs;

namespace LottoNumberRandomizer.Model.Queries;

public class GetLottoNumbersQuery : IQuery<Result<IEnumerable<LottoNumberDto>>>
{
    public int LastDrawsCount { get; init; }

    public LottoDateRange DateRange { get; init; }
}
