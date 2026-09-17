using System.ComponentModel.DataAnnotations;

namespace EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Clients;

public class CreateClientRequest
{
    [Required]
    [StringLength(10)]
    public string DocumentType { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }
}

