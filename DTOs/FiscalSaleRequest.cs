namespace Datecs.Rest.DTOs
{
    public class FiscalSaleRequest
    {
        public string? Text { get; set; }
        public int TaxGroup { get; set; }
        public decimal SinglePrice { get; set; }
        public decimal Quantity { get; set; } = 1.000m;
        public int DiscountType { get; set; } = 0;
        public string? DiscountValue { get; set; }
        public int Department { get; set; } = 1;
    }

}
