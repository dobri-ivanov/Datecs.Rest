using Datecs.Rest.DTOs;
using Datecs.Rest.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

[ApiController]
[Route("device")]
public class DeviceController : ControllerBase
{
    private readonly IFiscalService _fiscal;

    public DeviceController(IFiscalService fiscal)
    {
        _fiscal = fiscal;
    }

    [HttpPost("start")]
    public IActionResult Start([FromBody] DeviceStartRequest req)
    {
        try
        {
            _fiscal.Start(req.ComPort, req.BaudRate);
            return Ok(new { status = "CONNECTED" });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                error = "DEVICE_CONNECTION_FAILED",
                message = ex.Message
            });
        }
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        try
        {
            if (!_fiscal.IsConnected)
            {
                return Conflict(new
                {
                    error = "NOT_CONNECTED",
                    message = "Fiscal device is not started"
                });
            }

            bool ok = _fiscal.Ping();

            if (!ok)
            {
                return StatusCode(503, new
                {
                    error = "DEVICE_NOT_RESPONDING",
                    message = "Fiscal device does not respond"
                });
            }

            return Ok(new
            {
                status = "OK"
            });
        }
        catch (COMException)
        {
            return StatusCode(503, new
            {
                error = "COM_PORT_ERROR",
                message = "COM port not available"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "INTERNAL_ERROR",
                message = ex.Message
            });
        }
    }

    [HttpPost("receipt/open")]
    public IActionResult OpenFiscalReceipt([FromBody] FiscalOpenRequest req)
    {
        try
        {
            string slipNumber = _fiscal.OpenFiscalReceipt(req);

            return Ok(new
            {
                status = "OPENED",
                slipNumber
            });
        }
        catch (Exception ex)
        {
            return StatusCode(400, new
            {
                error = "FISCAL_OPEN_FAILED",
                message = ex.Message
            });
        }
    }

    [HttpPost("receipt/sale")]
    public IActionResult AddSale([FromBody] FiscalSaleRequest request)
    {
        try
        {
            _fiscal.AddSale(request);
            return Ok(new
            {
                status = "OK"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(400, new
            {
                error = "SALE_FAILED",
                message = ex.Message
            });
        }
    }



}
