namespace LottoNumberRandomizer.Model.Configuration;

public class LottoApiSettings
{
    public const string SectionName = "LottoApi";
    
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string GameType { get; set; } = "Lotto";
}
