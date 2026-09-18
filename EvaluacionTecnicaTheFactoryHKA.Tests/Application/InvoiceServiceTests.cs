using EvaluacionTecnicaTheFactoryHKA.Aplication.DTOs.Requests.Invoices;
using EvaluacionTecnicaTheFactoryHKA.Aplication.Services;
using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;
using EvaluacionTecnicaTheFactoryHKA.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace EvaluacionTecnicaTheFactoryHKA.Tests.Application;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly InvoiceService _sut;

    public InvoiceServiceTests()
    {
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _clientRepoMock = new Mock<IClientRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _sut = new InvoiceService(
            _invoiceRepoMock.Object,
            _clientRepoMock.Object,
            _productRepoMock.Object,
            _uowMock.Object);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithValidRequest_ShouldCreateInvoiceAndReturnId()
    {
        // Arrange
        var request = new CreateInvoiceRequest
        {
            ClientId = 1,
            Discount = 0,
            Details = new List<InvoiceDetailRequest>
            {
                new InvoiceDetailRequest { ProductId = 1, Quantity = 2 }
            }
        };

        var client = new Client { Id = 1, FirstName = "John", IsActive = true };
        var product = new Product { Id = 1, Name = "Prod1", UnitPrice = 50m, Stock = 10, IsActive = true };

        _clientRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(client);
        _productRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(new[] { product });
        _invoiceRepoMock.Setup(x => x.GenerateInvoiceNumberAsync()).ReturnsAsync("INV-000001");

        _invoiceRepoMock.Setup(x => x.AddAsync(It.IsAny<Invoice>()))
            .Callback<Invoice>(i => i.Id = 99) // Simulate DB identity assignment
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateInvoiceAsync(request);

        // Assert
        result.Should().Be(99);
        _invoiceRepoMock.Verify(x => x.AddAsync(It.IsAny<Invoice>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenClientNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new CreateInvoiceRequest { ClientId = 1 };
        _clientRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Client?)null);

        // Act
        var act = async () => await _sut.CreateInvoiceAsync(request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
