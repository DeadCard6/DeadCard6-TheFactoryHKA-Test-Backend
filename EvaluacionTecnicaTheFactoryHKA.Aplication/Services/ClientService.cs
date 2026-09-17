using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Clients;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;
using EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;

namespace EvaluacionTecnicaTheFactoryHKA.Aplication.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ClientService(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateAsync(CreateClientRequest request)
    {
        var client = new Client
        {
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            RegistrationDate = DateTime.UtcNow,
            IsActive = true
        };

        await _clientRepository.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();
        return client.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null) throw new NotFoundException(nameof(Client), id);
        
        client.IsActive = false;
        _clientRepository.Update(client);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<object> GetAllAsync(bool? isActive)
    {
        var clients = await _clientRepository.GetAllAsync(isActive);
        return clients.Select(c => new
        {
            c.Id,
            c.DocumentType,
            c.DocumentNumber,
            c.FirstName,
            c.LastName,
            c.Email,
            c.IsActive
        });
    }

    public async Task<object> GetByIdAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null) throw new NotFoundException(nameof(Client), id);
        return client;
    }

    public async Task UpdateAsync(int id, CreateClientRequest request)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null) throw new NotFoundException(nameof(Client), id);

        client.DocumentType = request.DocumentType;
        client.DocumentNumber = request.DocumentNumber;
        client.FirstName = request.FirstName;
        client.LastName = request.LastName;
        client.Email = request.Email;
        client.Phone = request.Phone;
        client.Address = request.Address;

        _clientRepository.Update(client);
        await _unitOfWork.SaveChangesAsync();
    }
}

