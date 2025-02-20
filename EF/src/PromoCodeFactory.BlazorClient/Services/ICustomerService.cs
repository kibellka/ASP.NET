using PromoCodeFactory.BlazorClient.Models;

namespace PromoCodeFactory.BlazorClient.Services;

public interface ICustomerService
{
    Task<ICollection<Customer>> GetAllCustomersAsync();

    Task<string> CreateCustomerAsync(Customer customer);

    Task<string> UpdateCustomerAsync(Customer customer);

    Task<string> DeleteCustomerAsync(string id);
}