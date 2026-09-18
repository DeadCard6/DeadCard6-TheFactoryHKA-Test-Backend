using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;

namespace EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

public class InvoiceDetail
{
    public int Id { get; set; }
    
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }

    // EF Core requires a parameterless constructor
    protected InvoiceDetail() { }

    internal InvoiceDetail(Product product, int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        
        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        UnitPrice = product.UnitPrice;
        Subtotal = quantity * product.UnitPrice;
    }
}
