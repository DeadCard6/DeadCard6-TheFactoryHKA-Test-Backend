namespace EvaluacionTecnicaTheFactoryHKA.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key) 
        : base($"Entity '{entityName}' with key '{key}' was not found.") { }
}

public class ClientInactiveException : DomainException
{
    public ClientInactiveException(int clientId) 
        : base($"Client with ID {clientId} is inactive and cannot generate invoices.") { }
}

public class ProductInactiveException : DomainException
{
    public ProductInactiveException(int productId) 
        : base($"Product with ID {productId} is inactive and cannot be sold.") { }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int requestedQuantity, int currentStock) 
        : base($"Insufficient stock for product '{productName}'. Requested {requestedQuantity} but only {currentStock} available.") { }
}

