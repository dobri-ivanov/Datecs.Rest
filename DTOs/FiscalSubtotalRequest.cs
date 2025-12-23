namespace Datecs.Rest.DTOs
{
    public class FiscalSubtotalRequest
    {

        public bool ToPrint { get; set; } = true;
        public bool ToDisplay { get; set; } = false;

        // ⚠️ По желание – обща отстъпка върху целия бон
        public int DiscountType { get; set; } = 0;
        public string? DiscountValue { get; set; }

    }
}
