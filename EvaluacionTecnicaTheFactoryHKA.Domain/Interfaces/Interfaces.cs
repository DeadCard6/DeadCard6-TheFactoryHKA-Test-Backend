using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

namespace EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
}

public interface ICategoryRepository { }

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids);
    void Update(Product product);
}

public interface IInvoiceRepository
{
    Task AddAsync(Invoice invoice);
    Task<string> GenerateInvoiceNumberAsync();
}

