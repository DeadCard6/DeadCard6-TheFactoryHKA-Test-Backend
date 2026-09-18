using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;
using FluentAssertions;

namespace EvaluacionTecnicaTheFactoryHKA.Tests.Domain;

public class InvoiceTests
{
    [Fact]
    public void AddDetail_WithValidProduct_ShouldAddDetailAndCalculateTotals()
    {
        // Arrange
        var invoice = new Invoice(clientId: 1, discount: 0);
        var product = new Product { Id = 1, Name = "Product 1", UnitPrice = 100m, Stock = 10, IsActive = true };

        // Act
        invoice.AddDetail(product, 2);

        // Assert
        invoice.Details.Should().HaveCount(1);
        invoice.Subtotal.Should().Be(200m);
        invoice.Tax.Should().Be(38m); // 19% of 200
        invoice.Total.Should().Be(238m);
        product.Stock.Should().Be(8); // Stock should be deducted
    }

    [Fact]
    public void AddDetail_WithInactiveProduct_ShouldThrowDomainException()
    {
        // Arrange
        var invoice = new Invoice(clientId: 1);
        var product = new Product { Id = 1, Name = "Product 1", UnitPrice = 100m, Stock = 10, IsActive = false };

        // Act
        var act = () => invoice.AddDetail(product, 1);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("*not active*");
    }

    [Fact]
    public void VoidInvoice_WhenStatusIsPending_ShouldChangeStatusAndReplenishStock()
    {
        // Arrange
        var invoice = new Invoice(clientId: 1);
        var product = new Product { Id = 1, Name = "Product 1", UnitPrice = 100m, Stock = 10, IsActive = true };
        invoice.AddDetail(product, 2);
        
        product.Stock.Should().Be(8);

        // Act
        invoice.VoidInvoice();

        // Assert
        invoice.Status.Should().Be("Voided");
        product.Stock.Should().Be(10); // Stock replenished
    }

    [Fact]
    public void Pay_WhenStatusIsPending_ShouldChangeStatusToPaid()
    {
        // Arrange
        var invoice = new Invoice(clientId: 1);

        // Act
        invoice.Pay();

        // Assert
        invoice.Status.Should().Be("Paid");
    }
}

