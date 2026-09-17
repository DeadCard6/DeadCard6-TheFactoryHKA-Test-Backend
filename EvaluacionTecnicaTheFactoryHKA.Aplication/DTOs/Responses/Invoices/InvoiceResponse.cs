namespace EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Responses.Invoices;

public class InvoiceResponse
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}

