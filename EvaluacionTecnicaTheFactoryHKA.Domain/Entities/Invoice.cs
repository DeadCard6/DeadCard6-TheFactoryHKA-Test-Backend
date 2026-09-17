namespace EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;

    public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
}

