using PromoCodeFactory.BlazorClient.Models;
using GrpcService.Client;
using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;

namespace PromoCodeFactory.BlazorClient.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly GrpcChannel _channel;
        private readonly Customers.CustomersClient _client;

        public CustomerService(GrpcChannel channel)
        {
            _channel = channel;
            _client = new Customers.CustomersClient(_channel);
        }

        public async Task<ICollection<Customer>> GetAllCustomersAsync()
        {
            var response = await _client.GetCustomersAsync(new Empty());

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
            var request = new CreateCustomerRequest
            {
                FirstName = customer.FirstName ?? string.Empty,
                LastName = customer.LastName ?? string.Empty,
                Email = customer.Email ?? string.Empty
            };

            var response = await _client.CreateCustomerAsync(request);

            return response.Id;
        }

        public async Task<string> UpdateCustomerAsync(Customer customer)
        {
            var request = new UpdateCustomerRequest
            {
                Id = customer.Id,
                FirstName = customer.FirstName ?? string.Empty,
                LastName = customer.LastName ?? string.Empty,
                Email = customer.Email ?? string.Empty
            };

            var response = await _client.UpdateCustomerAsync(request);

            return response.Id;
        }

        public async Task<string> DeleteCustomerAsync(string id)
        {
            var request = new DeleteCustomerRequest
            {
                Id = id
            };

            var response = await _client.DeleteCustomerAsync(request);

            return response.Id;
        }
    }
}
