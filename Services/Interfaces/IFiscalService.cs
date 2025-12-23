using Datecs.Rest.DTOs;

namespace Datecs.Rest.Services.Interfaces
{
    public interface IFiscalService
    {
        void Start(int comPort, int baudRate);
        bool Ping();
        string OpenFiscalReceipt(FiscalOpenRequest req);
        void AddSale(FiscalSaleRequest req);
        SubtotalResult Subtotal(FiscalSubtotalRequest req);
        void Total(FiscalTotalRequest req);
        string CloseFiscalReceipt();
        bool IsConnected { get; }
    }

}
