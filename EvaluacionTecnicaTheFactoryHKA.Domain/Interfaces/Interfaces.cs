using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;

namespace EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
    Task<IEnumerable<Client>> GetAllAsync(bool? isActive);
    Task AddAsync(Client client);
    void Update(Client client);
    void Delete(Client client);
}

public interface ICategoryRepository 
{ 
    Task<Category?> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task AddAsync(Category category);
    void Update(Category category);
}

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync(int? categoryId, bool? isActive, string? name);
    Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids);
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
}

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(int id);
    Task<IEnumerable<Invoice>> GetAllAsync(int? clientId, string? status, DateTime? startDate, DateTime? endDate);
    Task AddAsync(Invoice invoice);
    Task<string> GenerateInvoiceNumberAsync();
    void Update(Invoice invoice);
}

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
}

