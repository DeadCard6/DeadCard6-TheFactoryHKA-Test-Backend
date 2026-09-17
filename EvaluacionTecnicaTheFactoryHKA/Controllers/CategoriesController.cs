using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Categories;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EvaluacionTecnicaTheFactoryHKA.Controllers;

[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Gets a list of categories.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var id = await _categoryService.CreateAsync(request);
        return Created("", new { id = id });
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryRequest request)
    {
        await _categoryService.UpdateAsync(id, request);
        return NoContent();
    }
}

