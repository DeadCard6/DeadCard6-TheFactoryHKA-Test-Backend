using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices
            .Include(i => i.Details)
            .ThenInclude(d => d.Product)
            .Include(i => i.Client)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync(int? clientId, string? status, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Invoices
            .Include(i => i.Client)
            .AsQueryable();

        if (clientId.HasValue)
            query = query.Where(i => i.ClientId == clientId.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(i => i.Status == status);

        if (startDate.HasValue)
            query = query.Where(i => i.IssueDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(i => i.IssueDate <= endDate.Value);

        return await query.ToListAsync();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var count = await _context.Invoices.CountAsync();
        return $"INV-{count + 1:D6}";
    }

    public void Update(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
    }
}

