namespace EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public decimal PrecioUnitario { get; set; }
    public int Stock { get; set; }
    public bool Activo { get; set; }

    public ICollection<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();
}
