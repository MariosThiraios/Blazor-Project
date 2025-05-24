using BlazorApp.Services;
using BlazorApp.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _customerService.GetPaginatedCustomers(page, pageSize);

            return Ok(result);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetCustomer([FromRoute] Guid id)
        {
            var customer = await _customerService.GetCustomerById(id);

            return customer != null ? Ok(customer) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto customer)
        {
            if (customer == null)
                return BadRequest();

            await _customerService.CreateCustomer(customer);

            return Ok();
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> EditCustomer([FromRoute] Guid id, [FromBody] CustomerDto customer)
        {
            var edited = await _customerService.EditCustomer(id, customer);

            return edited ? Ok() : NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] Guid id)
        {
            var deleted = await _customerService.DeleteCustomer(id);

            return deleted ? Ok() : NotFound();
        }
    }
}