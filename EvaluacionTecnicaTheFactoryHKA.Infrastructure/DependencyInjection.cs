using EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvaluacionTecnicaTheFactoryHKA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces.IUnitOfWork, EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Repositories.UnitOfWork>();
        services.AddScoped<EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces.IUserRepository, EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Repositories.UserRepository>();
        
        services.AddScoped<EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces.IClientRepository, EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Repositories.ClientRepository>();
        services.AddScoped<EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces.IProductRepository, EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Repositories.ProductRepository>();
        services.AddScoped<EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces.IInvoiceRepository, EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Repositories.InvoiceRepository>();
        services.AddScoped<EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces.ICategoryRepository, EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence.Repositories.CategoryRepository>();

        return services;
    }
}

