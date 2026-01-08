namespace LottoNumberRandomizer.Model.DTOs;

public class RandomNumbersDto
{
    public int[] Numbers { get; set; } = [];

    public string GetRandomNumbers => Numbers.Length > 0 ? string.Join(" ,", Numbers) : string.Empty;
}
