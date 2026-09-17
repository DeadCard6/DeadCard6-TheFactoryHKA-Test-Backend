namespace EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

