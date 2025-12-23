namespace Datecs.Rest.DTOs
{
    public class FiscalTotalRequest
    {
        // 0 = cash, 1 = card, 2..5 = други според конфигурацията
        public int PaidMode { get; set; }

        // Ако е null → апаратът приема пълната сума
        public decimal? InputAmount { get; set; }

        // Само при плащане с пинпад
        // допустими стойности: "1" или "12"
        public string? PinPadPaidMode { get; set; }
    }

}
