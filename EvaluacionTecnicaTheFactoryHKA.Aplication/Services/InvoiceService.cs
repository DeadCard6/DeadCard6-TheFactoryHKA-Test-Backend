using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Invoices;
using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Responses.Invoices;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Interfaces;
using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;
using EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;

namespace EvaluacionTecnicaTheFactoryHKA.Aplication.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        var client = await _clientRepository.GetByIdAsync(request.ClientId);
        if (client == null) throw new NotFoundException(nameof(Client), request.ClientId);
        if (!client.IsActive) throw new DomainException($"Client '{client.FirstName}' is not active.");

        var invoice = new Invoice(request.ClientId, request.Discount);

        var productIds = request.Details.Select(d => d.ProductId).Distinct().ToList();
        var products = await _productRepository.GetByIdsAsync(productIds);

        foreach (var detailDto in request.Details)
        {
            var product = products.FirstOrDefault(p => p.Id == detailDto.ProductId);
            if (product == null) throw new NotFoundException(nameof(Product), detailDto.ProductId);

            // This will internally deduct stock and update totals
            invoice.AddDetail(product, detailDto.Quantity);
        }

        // Generate atomic Invoice Number from sequence
        var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync();
        invoice.SetInvoiceNumber(invoiceNumber);

        await _invoiceRepository.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return invoice.Id;
    }

    public async Task<IEnumerable<InvoiceResponse>> GetAllAsync(int? clientId, string? status, DateTime? startDate, DateTime? endDate)
    {
        var invoices = await _invoiceRepository.GetAllAsync(clientId, status, startDate, endDate);
        return invoices.Select(i => new InvoiceResponse
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            IssueDate = i.IssueDate,
            ClientName = $"{i.Client.FirstName} {i.Client.LastName}".Trim(),
            Subtotal = i.Subtotal,
            Tax = i.Tax,
            Discount = i.Discount,
            Total = i.Total,
            Status = i.Status
        });
    }

    public async Task<InvoiceResponse> GetByIdAsync(int id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null) throw new NotFoundException(nameof(Invoice), id);

        return new InvoiceResponse
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            ClientName = $"{invoice.Client.FirstName} {invoice.Client.LastName}".Trim(),
            Subtotal = invoice.Subtotal,
            Tax = invoice.Tax,
            Discount = invoice.Discount,
            Total = invoice.Total,
            Status = invoice.Status,
            Details = invoice.Details.Select(d => new InvoiceDetailResponse
            {
                Id = d.Id,
                ProductId = d.ProductId,
                ProductName = d.Product.Name,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal
            }).ToList()
        };
    }

    public async Task VoidAsync(int id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null) throw new NotFoundException(nameof(Invoice), id);
        
        // This will internally validate status and replenish stock
        invoice.VoidInvoice();

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task PayAsync(int id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null) throw new NotFoundException(nameof(Invoice), id);

        // Internally sets to Paid and validates
        invoice.Pay();

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync();
    }
}
