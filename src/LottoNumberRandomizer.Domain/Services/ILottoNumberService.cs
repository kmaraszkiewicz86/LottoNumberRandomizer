using FluentResults;
using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Queries;

namespace LottoNumberRandomizer.Infrastructure.Services;

public interface ILottoNumberService
{
    Task<Result<IEnumerable<LottoNumberDto>>> GetLatest(GetLottoNumbersQuery query);
    Task<Result<IEnumerable<RandomNumbersDto>>> GetRandomNumbers(GetRandomNumbersQuery query);
}
