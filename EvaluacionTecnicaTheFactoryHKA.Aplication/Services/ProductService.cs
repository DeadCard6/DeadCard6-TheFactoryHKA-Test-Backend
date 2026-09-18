using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Products;
using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Responses.Products;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;
using EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;

namespace EvaluacionTecnicaTheFactoryHKA.Aplication.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateAsync(CreateProductRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null) throw new NotFoundException(nameof(Category), request.CategoryId);

        var product = new Product
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            UnitPrice = request.UnitPrice,
            Stock = request.Stock,
            IsActive = true
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new NotFoundException(nameof(Product), id);
        
        product.IsActive = false;
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync(int? categoryId, bool? isActive, string? name)
    {
        var products = await _productRepository.GetAllAsync(categoryId, isActive, name);
        return products.Select(p => new ProductResponse
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            CategoryId = p.CategoryId,
            UnitPrice = p.UnitPrice,
            Stock = p.Stock,
            IsActive = p.IsActive
        });
    }

    public async Task<ProductResponse> GetByIdAsync(int id)
    {
        var p = await _productRepository.GetByIdAsync(id);
        if (p == null) throw new NotFoundException(nameof(Product), id);
        return new ProductResponse
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            CategoryId = p.CategoryId,
            UnitPrice = p.UnitPrice,
            Stock = p.Stock,
            IsActive = p.IsActive
        };
    }

    public async Task UpdateAsync(int id, CreateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) throw new NotFoundException(nameof(Product), id);

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null) throw new NotFoundException(nameof(Category), request.CategoryId);

        product.Code = request.Code;
        product.Name = request.Name;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;
        product.UnitPrice = request.UnitPrice;
        product.Stock = request.Stock;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();
    }
}
