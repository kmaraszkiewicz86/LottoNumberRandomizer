using LottoNumberRandomizer.Model.DTOs;

namespace LottoNumberRandomizer.Infrastructure.Services;

public interface ILottoNumberService
{
    Task<IEnumerable<LottoNumberDto>> GenerateRandomNumbersAsync();
}
