using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services.Strategies;

// Strategy Pattern — conservative strategy: always keeps 20% of stock in reserve
public class ReserveAllocationStrategy : IAllocationStrategy
{
    private const decimal ReservePercent = 0.20m;

    public Task AllocateAsync(Resource resource, decimal quantity)
    {
        if (resource.Quantity < quantity)
            throw new InvalidOperationException(
                $"Insufficient stock. Available: {resource.Quantity} {resource.Unit}.");

        var minimumReserve    = resource.Quantity * ReservePercent;
        var remainingAfterUse = resource.Quantity - quantity;

        if (remainingAfterUse < minimumReserve)
            throw new InvalidOperationException(
                $"Allocation denied: must maintain 20% reserve ({minimumReserve:F2} {resource.Unit}). " +
                $"Maximum allocatable: {resource.Quantity - minimumReserve:F2} {resource.Unit}.");

        resource.Quantity -= quantity;
        return Task.CompletedTask;
    }
}
