namespace LottoNumberRandomizer.Domain.Entities;

public class LottoNumber
{
    public int Number { get; set; }
    public int Count { get; set; }

    public LottoNumber(int number, int count)
    {
        Number = number;
        Count = count;
    }
}
