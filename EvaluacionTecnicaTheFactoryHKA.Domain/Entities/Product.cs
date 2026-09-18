using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;

namespace EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }

    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public void DeductStock(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        if (Stock < quantity) throw new InsufficientStockException(Name, quantity, Stock);
        
        Stock -= quantity;
    }

    public void ReplenishStock(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        Stock += quantity;
    }

    // For EF Core / Initialization
    public void SetInitialStock(int stock)
    {
        Stock = stock;
    }
}
