using LottoNumberRandomizer.Model.DTOs;
using LottoNumberRandomizer.Model.Queries;

namespace LottoNumberRandomizer.Infrastructure.Services;

public interface ILottoNumberService
{
    Task<IEnumerable<LottoNumberDto>> GetLatest(GetLottoNumbersQuery query);
}
