using LottoNumberRandomizer.Model.Enums;

namespace LottoNumberRandomizer.Presentation.Models
{
    public sealed class LottoDateRangeOption
    {
        public LottoDateRange Value { get; }
        public string DisplayName { get; }

        public LottoDateRangeOption(LottoDateRange value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }
    }
}
