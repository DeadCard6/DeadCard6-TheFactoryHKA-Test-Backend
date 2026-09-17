using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Clients;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EvaluacionTecnicaTheFactoryHKA.Controllers;

[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// Gets a paginated list of clients.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
    {
        var result = await _clientService.GetAllAsync(isActive);
        return Ok(result);
    }

    /// <summary>
    /// Gets a client by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _clientService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        var id = await _clientService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = id }, new { id = id });
    }

    /// <summary>
    /// Updates an existing client.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateClientRequest request)
    {
        await _clientService.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Soft deletes a client.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _clientService.DeleteAsync(id);
        return NoContent();
    }
}

