using BlazorApp.Models;
using BlazorApp.ViewModels;
using BlazorApp.ViewModels.Customer;

namespace BlazorApp.Services
{
    public interface ICustomerService
    {
        Task<PaginatedResultDto<CustomerDto>> GetPaginatedCustomers(int page, int pageSize);
        Task<CustomerDto?> GetCustomerById(Guid id);
        Task CreateCustomer(CustomerDto customerDto);
        Task<bool> EditCustomer(Guid id, CustomerDto customerDto);
        Task<bool> DeleteCustomer(Guid id);
    }
}