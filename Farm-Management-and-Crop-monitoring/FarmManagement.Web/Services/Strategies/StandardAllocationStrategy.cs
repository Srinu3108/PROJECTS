using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services.Strategies;

// Strategy Pattern — default strategy: deducts stock immediately, no reserve floor
public class StandardAllocationStrategy : IAllocationStrategy
{
    public Task AllocateAsync(Resource resource, decimal quantity)
    {
        if (resource.Quantity < quantity)
            throw new InvalidOperationException(
                $"Insufficient stock. Available: {resource.Quantity} {resource.Unit}.");

        resource.Quantity -= quantity;
        return Task.CompletedTask;
    }
}
