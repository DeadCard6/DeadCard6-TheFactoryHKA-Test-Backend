using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _context.Clients.FindAsync(id);
    }

    public async Task<IEnumerable<Client>> GetAllAsync(bool? isActive)
    {
        var query = _context.Clients.AsQueryable();
        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }
        return await query.ToListAsync();
    }

    public async Task AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
    }

    public void Update(Client client)
    {
        _context.Clients.Update(client);
    }

    public void Delete(Client client)
    {
        _context.Clients.Remove(client);
    }
}

