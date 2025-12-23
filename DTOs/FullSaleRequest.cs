namespace Datecs.Rest.DTOs
{
    using System.Collections.Generic;

    public class FullSaleRequest
    {
        public List<SaleRequest> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}
