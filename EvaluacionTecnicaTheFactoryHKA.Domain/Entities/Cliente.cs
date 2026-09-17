namespace EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string? Apellidos { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}
