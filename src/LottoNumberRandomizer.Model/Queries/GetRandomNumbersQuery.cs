using FluentResults;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Enums;
using SimpleCqrs;

namespace LottoNumberRandomizer.Model.Queries;

public class GetRandomNumbersQuery : IQuery<Result<IEnumerable<RandomNumbersDto>>>
{
    public int NumberCount { get; init; }
    public int TicketCount { get; init; }

    public LottoDateRange DateRange { get; init; }
}
