using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Invoices;
using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Responses.Invoices;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EvaluacionTecnicaTheFactoryHKA.Controllers;

[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    /// <summary>
    /// Creates a new invoice and processes the sale.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
    {
        var response = await _invoiceService.CreateInvoiceAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Gets a paginated and filtered list of invoices.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? clientId, [FromQuery] string? status, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _invoiceService.GetAllAsync(clientId, status, startDate, endDate);
        return Ok(result);
    }

    /// <summary>
    /// Gets the complete details of an invoice.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _invoiceService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Voids an invoice (Changes status to 'Voided').
    /// </summary>
    /// <remarks>
    /// Business rule: Voiding an invoice DOES replenish product stock. 
    /// This responsibility belongs to the Application Service.
    /// </remarks>
    [HttpPut("{id}/void")]
    public async Task<IActionResult> Void(int id)
    {
        await _invoiceService.VoidAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Marks an invoice as paid.
    /// </summary>
    [HttpPut("{id}/pay")]
    public async Task<IActionResult> Pay(int id)
    {
        await _invoiceService.PayAsync(id);
        return NoContent();
    }
}

