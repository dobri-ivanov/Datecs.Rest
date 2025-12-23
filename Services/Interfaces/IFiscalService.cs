using Datecs.Rest.DTOs;

namespace Datecs.Rest.Services.Interfaces
{
    public interface IFiscalService
    {
        void Start(int comPort, int baudRate);

        bool Ping();

        string OpenFiscalReceipt(FiscalOpenRequest req);
        void AddSale(FiscalSaleRequest req);
        //void Subtotal();
        //void Total(int paidMode, decimal amount);
        //void CloseFiscalReceipt();
        bool IsConnected { get; }
    }

}
