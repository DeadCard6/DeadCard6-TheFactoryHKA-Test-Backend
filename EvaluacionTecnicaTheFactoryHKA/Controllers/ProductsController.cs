using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Products;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EvaluacionTecnicaTheFactoryHKA.Controllers;

[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Gets a filtered list of products.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? categoryId, [FromQuery] bool? isActive, [FromQuery] string? name)
    {
        var result = await _productService.GetAllAsync(categoryId, isActive, name);
        return Ok(result);
    }

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var id = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = id }, new { id = id });
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProductRequest request)
    {
        await _productService.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Soft deletes a product.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Reactivates a soft-deleted product.
    /// </summary>
    [HttpPatch("{id}/reactivate")]
    public async Task<IActionResult> Reactivate(int id)
    {
        await _productService.ReactivateAsync(id);
        return NoContent();
    }
}

