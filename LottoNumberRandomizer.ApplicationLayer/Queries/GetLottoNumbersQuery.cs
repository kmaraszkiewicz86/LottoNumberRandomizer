using LottoNumberRandomizer.Model.DTOs;
using SimpleCqrs;

namespace LottoNumberRandomizer.ApplicationLayer.Queries;

public class GetLottoNumbersQuery : IQuery<IEnumerable<LottoNumberDto>>
{
}
