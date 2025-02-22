using PromoCodeFactory.BlazorClient.Models;
using GrpcService.Client;
using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;

namespace PromoCodeFactory.BlazorClient.Services
{
    public class CustomerService(GrpcChannel channel) : ICustomerService
    {
        GrpcChannel _channel = channel;

        public async Task<ICollection<Customer>> GetAllCustomersAsync()
        {
            var client = new Customers.CustomersClient(_channel);

            var response = await client.GetCustomersAsync(new Empty());

            var result = response.Customers.Select(c => new Customer
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email
                })
                .ToList();

            return result;
        }

        public async Task<string> CreateCustomerAsync(Customer customer)
        {
            var client = new Customers.CustomersClient(_channel);

            var request = new CreateCustomerRequest
            {
                FirstName = customer.FirstName ?? string.Empty,
                LastName = customer.LastName ?? string.Empty,
                Email = customer.Email ?? string.Empty
            };

            var response = await client.CreateCustomerAsync(request);

            return response.Id;
        }

        public async Task<string> UpdateCustomerAsync(Customer customer)
        {
            var client = new Customers.CustomersClient(_channel);

            var request = new UpdateCustomerRequest
            {
                Id = customer.Id,
                FirstName = customer.FirstName ?? string.Empty,
                LastName = customer.LastName ?? string.Empty,
                Email = customer.Email ?? string.Empty
            };

            var response = await client.UpdateCustomerAsync(request);

            return response.Id;
        }

        public async Task<string> DeleteCustomerAsync(string id)
        {
            var client = new Customers.CustomersClient(_channel);

            var request = new DeleteCustomerRequest
            {
                Id = id
            };

            var response = await client.DeleteCustomerAsync(request);

            return response.Id;
        }
    }
}
