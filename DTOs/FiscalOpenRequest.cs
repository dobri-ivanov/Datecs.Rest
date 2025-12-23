namespace Datecs.Rest.DTOs
{
    public class FiscalOpenRequest
    {
        public int OperatorNumber { get; set; } = 1;
        public string OperatorPassword { get; set; } = "1";
        public string? UNP { get; set; }              
        public int TillNumber { get; set; } = 1;     
        public bool Invoice { get; set; } = false;  
    }
}
