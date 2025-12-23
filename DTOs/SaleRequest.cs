namespace Datecs.Rest.DTOs
{
    public class SaleRequest
    {
        public string Text { get; set; } = "";
        public int TaxGroup { get; set; } = 1;
        public decimal Price { get; set; }
        public decimal Quantity { get; set; } = 1;
    }

}
