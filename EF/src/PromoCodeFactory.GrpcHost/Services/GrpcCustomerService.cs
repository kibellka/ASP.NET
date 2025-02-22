using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService.Server;
using PromoCodeFactory.Core.Services.Abstractions;
using PromoCodeFactory.Core.Services.Contracts.Customer;

namespace PromoCodeFactory.GrpcHost.Services;

public class GrpcCustomerService(ICustomerService customerService) : Customers.CustomersBase
{
    private ICustomerService _customerService = customerService;

    public async override Task<CustomerListResponse> GetCustomers(Empty request, ServerCallContext context)
    {
        var data = await _customerService.GetAllAsync();

        var customers = data.Select(d => new CustomerShortResponse
        {
            Id = d.Id.ToString(),
            FirstName = d.FirstName,
            LastName = d.LastName,
            Email = d.Email,
        });

        var response = new CustomerListResponse();
        response.Customers.AddRange(customers);

        return response;
    }

    public async override Task<CustomerResponse> GetCustomer(GetCustomerRequest request, ServerCallContext context)
    {
        var id = new Guid(request.Id);

        var customer = await _customerService.GetByIdAsync(id);

        if (customer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
        }

        var response = new CustomerResponse()
        {
            Id = customer.Id.ToString(),
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email
        };

        response.Preferences.AddRange(customer.Preferences.Select(p => new GrpcService.Server.CustomerPreference
        {
           Id = p.Id.ToString(),
           Name = p.Name
        }));

        response.PromoCodes.AddRange(customer.PromoCodes.Select(p => new GrpcService.Server.CustomerPromoCode
        {
            Id = p.Id.ToString(),
            Code = p.Code,
            ServiceInfo = p.ServiceInfo,
            BeginDate = Timestamp.FromDateTime(p.BeginDate),
            EndDate = Timestamp.FromDateTime(p.EndDate),
            PartnerName = p.PartnerName
        }));

        return response;
    }

    public async override Task<CreateCustomerResponse> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
    {
        var customer = new CustomerCreateOrEditDto
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        var result = await _customerService.CreateAsync(customer);

        var response = new CreateCustomerResponse
        {
            Id = result.ToString()
        };

        return response;
    }

    public async override Task<CustomerShortResponse> UpdateCustomer(UpdateCustomerRequest request, ServerCallContext context)
    {
        var id = new Guid(request.Id);

        var customer = new CustomerCreateOrEditDto
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        var result = await _customerService.UpdateAsync(id, customer);

        if (result == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));

        var response = new CustomerShortResponse
        {
            Id = result.Id.ToString(),
            FirstName = result.FirstName,
            LastName = result.LastName,
            Email = result.Email
        };

        return response;
    }

    public async override Task<DeleteCustomerResponse> DeleteCustomer(DeleteCustomerRequest request, ServerCallContext context)
    {
        var id = new Guid(request.Id);
        var result = await _customerService.DeleteAsync(id);

        if (!result)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Customer not found"));
        }

        return new DeleteCustomerResponse { Id = request.Id };
    }
}