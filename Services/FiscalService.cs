using Datecs.Rest.DTOs;
using Datecs.Rest.Services.Interfaces;
using FP3530;
using System.Runtime.InteropServices;

public class FiscalDeviceException : Exception
{
    public int ErrorCode { get; }

    public FiscalDeviceException(string message, int errorCode = -1)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class FiscalService : IFiscalService
{
    private CFD_BGR _fp;
    public bool IsConnected { get; private set; }

    private readonly object _sync = new();

    public void Start(int comPort, int baudRate)
    {
        lock (_sync)
        {
            if (IsConnected)
            {
                if (Ping())
                    return;
                SafeDisconnect();
            }

            try
            {
                _fp = new CFD_BGR();

                _fp.set_TransportType(TTransportProtocol.ctc_RS232);
                _fp.set_RS232(comPort, baudRate);

                int rc = _fp.OPEN_CONNECTION();
                if (rc != 0)
                    throw new Exception(_fp.get_ErrorMessageByCode(rc));
                if (!Ping())
                    throw new Exception("DEVICE_NOT_RESPONDING");

                IsConnected = true;
            }
            catch (COMException)
            {
                SafeDisconnect();
                throw new Exception("COM_PORT_ERROR");
            }
            catch
            {
                SafeDisconnect();
                throw;
            }
        }
    }

    private bool CheckConnection()
    {
        try
        {
            string output = string.Empty;

            int rc = _fp.execute_Command(
                45,
                "",
                ref output
            );

            return rc == 0;
        }
        catch
        {
            return false;
        }
    }

    private void SafeDisconnect()
    {
        try
        {
            if (_fp != null)
                _fp.CLOSE_CONNECTION();
        }
        catch { }
        finally
        {
            _fp = null;
            IsConnected = false;
        }
    }

    public bool Ping()
    {
        try
        {
            string output = "";
            int rc = _fp.execute_Command(45, "", ref output);

            return rc == 0;
        }
        catch
        {
            return false;
        }
    }
    private void Log(string msg)
    {
        Console.WriteLine($"[FISCAL] {DateTime.Now:HH:mm:ss} {msg}");
    }


    public string OpenFiscalReceipt(FiscalOpenRequest req)
    {
        if (_fp == null)
            throw new Exception("DEVICE_NOT_STARTED");

        if (!_fp.connected_ToDevice)
            throw new Exception("DEVICE_NOT_CONNECTED");

        string errorCode = "";
        string slipNumber = "";

        const string cmd = "048_receipt_Fiscal_Open";

        int rc;

        rc = _fp.set_InputParam_ByName(cmd, "OperatorNumber", req.OperatorNumber.ToString());
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.set_InputParam_ByName(cmd, "OperatorPassword", req.OperatorPassword);
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.set_InputParam_ByName(cmd, "UNP", req.UNP);
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.set_InputParam_ByName(cmd, "TillNumber", req.TillNumber.ToString());
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        if (req.Invoice)
        {
            rc = _fp.set_InputParam_ByName(cmd, "Invoice", "1");
            if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));
        }

        rc = _fp.execute_Command_ByName(cmd);
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.get_OutputParam_ByName(cmd, "ErrorCode", ref errorCode);
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        if (int.Parse(errorCode) != 0)
            throw new Exception(_fp.get_ErrorMessageByCode(int.Parse(errorCode)));

        rc = _fp.get_OutputParam_ByName(cmd, "SlipNumber", ref slipNumber);
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        return slipNumber;
    }


    public void AddSale(FiscalSaleRequest req)
    {
        if (_fp == null)
            throw new Exception("DEVICE_NOT_STARTED");

        if (!_fp.connected_ToDevice)
            throw new Exception("DEVICE_NOT_CONNECTED");

        const string cmd = "049_receipt_Sale";

        string errorCode = "";
        string slipNumber = "";
        int rc;

        rc = _fp.set_InputParam_ByName(cmd, "TextRow1", req.Text);
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.set_InputParam_ByName(cmd, "TaxGroup", req.TaxGroup.ToString());
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.set_InputParam_ByName(
            cmd,
            "SinglePrice",
            req.SinglePrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
        );
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.set_InputParam_ByName(
            cmd,
            "Quantity",
            req.Quantity.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)
        );
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        // ⚠️ Discount САМО ако има
        if (req.DiscountType > 0)
        {
            rc = _fp.set_InputParam_ByName(cmd, "Discount_Type", req.DiscountType.ToString());
            if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

            rc = _fp.set_InputParam_ByName(cmd, "Discount_Value", req.DiscountValue);
            if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));
        }

        rc = _fp.set_InputParam_ByName(cmd, "Department", req.Department.ToString());
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.execute_Command_ByName(cmd);
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        rc = _fp.get_OutputParam_ByName(cmd, "ErrorCode", ref errorCode);
        if (rc != 0) throw new Exception(_fp.get_ErrorMessageByCode(rc));

        if (int.Parse(errorCode) != 0)
            throw new Exception(_fp.get_ErrorMessageByCode(int.Parse(errorCode)));

        // SlipNumber е optional, но ако ти трябва:
        _fp.get_OutputParam_ByName(cmd, "SlipNumber", ref slipNumber);
    }
}
