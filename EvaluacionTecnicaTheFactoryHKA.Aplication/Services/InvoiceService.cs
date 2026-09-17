using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Invoices;
using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Responses.Invoices;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;
using EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;

namespace EvaluacionTecnicaTheFactoryHKA.Aplication.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IClientRepository _clientRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InvoiceService(
        IClientRepository clientRepository,
        IProductRepository productRepository,
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _productRepository = productRepository;
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        var client = await _clientRepository.GetByIdAsync(request.ClientId);
        if (client == null)
            throw new NotFoundException(nameof(Client), request.ClientId);
        
        if (!client.IsActive)
            throw new ClientInactiveException(client.Id);

        if (request.Details == null || !request.Details.Any())
            throw new DomainException("Invoice must contain at least one product.");

        var productIds = request.Details.Select(d => d.ProductId).Distinct();
        var productsDb = (await _productRepository.GetByIdsAsync(productIds)).ToDictionary(p => p.Id);

        var newInvoice = new Invoice
        {
            ClientId = client.Id,
            Discount = request.Discount,
            Status = "Pending",
            IssueDate = DateTime.UtcNow
        };

        decimal generalSubtotal = 0;

        foreach (var detailReq in request.Details)
        {
            if (detailReq.Quantity <= 0)
                throw new DomainException($"Quantity for product {detailReq.ProductId} must be greater than zero.");

            if (!productsDb.TryGetValue(detailReq.ProductId, out var product))
                throw new NotFoundException(nameof(Product), detailReq.ProductId);

            if (!product.IsActive)
                throw new ProductInactiveException(product.Id);

            if (detailReq.Quantity > product.Stock)
                throw new InsufficientStockException(product.Name, detailReq.Quantity, product.Stock);

            var lineSubtotal = detailReq.Quantity * product.UnitPrice;
            generalSubtotal += lineSubtotal;

            product.Stock -= detailReq.Quantity;
            _productRepository.Update(product); 

            newInvoice.Details.Add(new InvoiceDetail
            {
                ProductId = product.Id,
                Quantity = detailReq.Quantity,
                UnitPrice = product.UnitPrice 
            });
        }

        const decimal TAX_RATE = 0.19m;
        newInvoice.Subtotal = generalSubtotal;
        
        if (newInvoice.Discount > newInvoice.Subtotal)
            throw new DomainException("Discount cannot be greater than invoice subtotal.");

        var taxableBase = newInvoice.Subtotal - newInvoice.Discount;
        newInvoice.Tax = taxableBase * TAX_RATE;
        newInvoice.Total = taxableBase + newInvoice.Tax;

        newInvoice.InvoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync();

        await _invoiceRepository.AddAsync(newInvoice);
        await _unitOfWork.SaveChangesAsync(); 

        return new InvoiceResponse
        {
            Id = newInvoice.Id,
            InvoiceNumber = newInvoice.InvoiceNumber,
            Total = newInvoice.Total,
            Status = newInvoice.Status
        };
    }

    public Task<object> GetAllAsync(int? clientId, string? status, DateTime? startDate, DateTime? endDate) => throw new NotImplementedException();
    public Task<object> GetByIdAsync(int id) => throw new NotImplementedException();
    public Task VoidAsync(int id) => throw new NotImplementedException();
    public Task PayAsync(int id) => throw new NotImplementedException();
}

