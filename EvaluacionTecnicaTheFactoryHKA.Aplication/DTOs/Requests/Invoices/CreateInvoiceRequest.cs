using System.ComponentModel.DataAnnotations;

namespace EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Invoices;

public class CreateInvoiceRequest
{
    [Required]
    public int ClientId { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Discount cannot be negative")]
    public decimal Discount { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "Invoice must have at least one detail line.")]
    public List<InvoiceDetailRequest> Details { get; set; } = new List<InvoiceDetailRequest>();
}

public class InvoiceDetailRequest
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}

