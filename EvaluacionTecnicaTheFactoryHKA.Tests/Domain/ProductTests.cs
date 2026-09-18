using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;
using FluentAssertions;

namespace EvaluacionTecnicaTheFactoryHKA.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void DeductStock_WhenStockIsSufficient_ShouldDecreaseStock()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Stock = 10
        };

        // Act
        product.DeductStock(3);

        // Assert
        product.Stock.Should().Be(7);
    }

    [Fact]
    public void DeductStock_WhenQuantityIsZeroOrLess_ShouldThrowDomainException()
    {
        // Arrange
        var product = new Product { Name = "Test", Stock = 10 };

        // Act
        var actZero = () => product.DeductStock(0);
        var actNegative = () => product.DeductStock(-1);

        // Assert
        actZero.Should().Throw<DomainException>().WithMessage("Quantity must be greater than zero.");
        actNegative.Should().Throw<DomainException>().WithMessage("Quantity must be greater than zero.");
    }

    [Fact]
    public void DeductStock_WhenStockIsInsufficient_ShouldThrowInsufficientStockException()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Stock = 5
        };

        // Act
        var act = () => product.DeductStock(10);

        // Assert
        act.Should().Throw<InsufficientStockException>();
    }

    [Fact]
    public void ReplenishStock_WhenQuantityIsValid_ShouldIncreaseStock()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Stock = 5
        };

        // Act
        product.ReplenishStock(5);

        // Assert
        product.Stock.Should().Be(10);
    }

    [Fact]
    public void ReplenishStock_WhenQuantityIsZeroOrLess_ShouldThrowDomainException()
    {
        // Arrange
        var product = new Product { Name = "Test", Stock = 5 };

        // Act
        var act = () => product.ReplenishStock(0);

        // Assert
        act.Should().Throw<DomainException>();
    }
}

