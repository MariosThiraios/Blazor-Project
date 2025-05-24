using BlazorApp.Data;
using BlazorApp.Models;
using BlazorApp.ViewModels;
using BlazorApp.ViewModels.Customer;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Services
{
    // Our logic in this service has to change and refactored. We need to add AutoMapper and move the logic we take the data from DB to a repository.
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResultDto<CustomerDto>> GetPaginatedCustomers(int page, int pageSize)
        {
            var totalCount = await _context.Customers.CountAsync();

            var customers = await _context.Customers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var customerDtos = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                ContactName = c.ContactName,
                Address = c.Address,
                City = c.City,
                Region = c.Region,
                PostalCode = c.PostalCode,
                Country = c.Country,
                Phone = c.Phone
            }).ToList();

            return new PaginatedResultDto<CustomerDto>
            {
                Items = customerDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<CustomerDto?> GetCustomerById(Guid id)
        {
            var customer =  await _context.Customers.FindAsync(id);

            if (customer == null) return null;

            return new CustomerDto
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                ContactName = customer.ContactName,
                Address = customer.Address,
                City = customer.City,
                Region = customer.Region,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                Phone = customer.Phone
            };
        }

        public async Task CreateCustomer(CustomerDto customerDto)
        {
            var newCustomer = new Customer
            {
                CompanyName = customerDto.CompanyName,
                ContactName = customerDto.ContactName,
                Address = customerDto.Address,
                City = customerDto.City,
                Region = customerDto.Region,
                PostalCode = customerDto.PostalCode,
                Country = customerDto.Country,
                Phone = customerDto.Phone
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EditCustomer(Guid id, CustomerDto customerDto)
        {
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null) return false;

            existingCustomer.CompanyName = customerDto.CompanyName;
            existingCustomer.ContactName = customerDto.ContactName;
            existingCustomer.Address = customerDto.Address;
            existingCustomer.City = customerDto.City;
            existingCustomer.Region = customerDto.Region;
            existingCustomer.PostalCode = customerDto.PostalCode;
            existingCustomer.Country = customerDto.Country;
            existingCustomer.Phone = customerDto.Phone;

            _context.Customers.Update(existingCustomer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCustomer(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}