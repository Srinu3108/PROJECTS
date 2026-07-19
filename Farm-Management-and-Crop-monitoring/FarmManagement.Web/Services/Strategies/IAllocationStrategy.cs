using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Services.Strategies;

// Strategy Pattern — defines the contract for any resource allocation rule
public interface IAllocationStrategy
{
    Task AllocateAsync(Resource resource, decimal quantity);
}
