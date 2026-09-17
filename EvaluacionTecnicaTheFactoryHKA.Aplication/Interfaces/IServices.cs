using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Categories;
using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Clients;
using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Invoices;
using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Products;
using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Responses.Invoices;

namespace EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;

public interface IClientService
{
    Task<object> GetAllAsync(bool? isActive);
    Task<object> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateClientRequest request);
    Task UpdateAsync(int id, CreateClientRequest request);
    Task DeleteAsync(int id);
}

public interface ICategoryService
{
    Task<object> GetAllAsync();
    Task<int> CreateAsync(CreateCategoryRequest request);
    Task UpdateAsync(int id, CreateCategoryRequest request);
}

public interface IProductService
{
    Task<object> GetAllAsync(int? categoryId, bool? isActive, string? name);
    Task<object> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateProductRequest request);
    Task UpdateAsync(int id, CreateProductRequest request);
    Task DeleteAsync(int id);
}

public interface IInvoiceService
{
    Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request);
    Task<object> GetAllAsync(int? clientId, string? status, DateTime? startDate, DateTime? endDate);
    Task<object> GetByIdAsync(int id);
    Task VoidAsync(int id);
    Task PayAsync(int id);
}

