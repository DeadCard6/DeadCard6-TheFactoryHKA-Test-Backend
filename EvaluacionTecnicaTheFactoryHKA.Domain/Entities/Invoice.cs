using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;

namespace EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    
    public DateTime IssueDate { get; private set; }
    
    public decimal Subtotal { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }
    
    public string Status { get; private set; } = "Pending";

    private readonly List<InvoiceDetail> _details = new();
    public IReadOnlyCollection<InvoiceDetail> Details => _details.AsReadOnly();

    public Invoice() { }

    public Invoice(int clientId, decimal discount = 0)
    {
        ClientId = clientId;
        IssueDate = DateTime.UtcNow;
        Status = "Pending";
        Discount = discount;
    }

    public void SetInvoiceNumber(string invoiceNumber)
    {
        InvoiceNumber = invoiceNumber;
    }

    public void AddDetail(Product product, int quantity)
    {
        if (Status != "Pending") throw new DomainException("Cannot add details to an invoice that is not pending.");
        if (!product.IsActive) throw new DomainException($"Product '{product.Name}' is not active.");
        
        product.DeductStock(quantity);
        
        var detail = new InvoiceDetail(product, quantity);
        _details.Add(detail);

        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        Subtotal = _details.Sum(d => d.Subtotal);
        Tax = Subtotal * 0.19m; // 19% Tax hardcoded based on original logic
        Total = Subtotal + Tax - Discount;
    }

    public void VoidInvoice()
    {
        if (Status == "Voided") throw new DomainException("Invoice is already voided.");
        
        foreach (var detail in _details)
        {
            detail.Product.ReplenishStock(detail.Quantity);
        }

        Status = "Voided";
    }

    public void Pay()
    {
        if (Status != "Pending") throw new DomainException($"Cannot pay an invoice with status '{Status}'.");
        Status = "Paid";
    }
}
