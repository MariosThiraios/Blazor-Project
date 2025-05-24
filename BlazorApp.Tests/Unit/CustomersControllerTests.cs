using BlazorApp.Controllers;
using BlazorApp.Services;
using BlazorApp.ViewModels;
using BlazorApp.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlazorApp.Tests.Unit
{
    public class CustomersControllerTests
    {
        [Fact]
        public async Task GetPaginatedCustomers_ReturnsOkWithPaginatedCustomerDto()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();

            int page = 1;
            int pageSize = 10;

            var expectedCustomers = new List<CustomerDto>
            {
                new() {Id = Guid.NewGuid(),CompanyName = "Test1",ContactName = "John Doe",Address = "111 Main St",City = "New York",Region = "NY",PostalCode = "10001",Country = "USA",Phone = "123-456-7890"},
                new() {Id = Guid.NewGuid(),CompanyName = "Test2",ContactName = "John Doe",Address = "222 Main St",City = "New York",Region = "NY",PostalCode = "10001",Country = "USA",Phone = "123-456-7890"},
                new() {Id = Guid.NewGuid(),CompanyName = "Test3",ContactName = "John Doe",Address = "333 Main St",City = "New York",Region = "NY",PostalCode = "10001",Country = "USA",Phone = "123-456-7890"},
                new() {Id = Guid.NewGuid(),CompanyName = "Test4",ContactName = "John Doe",Address = "444 Main St",City = "New York",Region = "NY",PostalCode = "10001",Country = "USA",Phone = "123-456-7890"},
            };

            var expectedResult = new PaginatedResultDto<CustomerDto>
            {
                Items = expectedCustomers,
                TotalCount = expectedCustomers.Count,
                Page = page,
                PageSize = pageSize
            };

            mockService.Setup(s => s.GetPaginatedCustomers(page, pageSize))
                       .ReturnsAsync(expectedResult);

            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.GetPaginatedCustomers(page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<PaginatedResultDto<CustomerDto>>(okResult.Value);

            Assert.Equal(expectedResult.TotalCount, actual.TotalCount);
            Assert.Equal(expectedResult.Page, actual.Page);
            Assert.Equal(expectedResult.PageSize, actual.PageSize);
            Assert.Equal(expectedResult.Items.Count, actual.Items.Count);
        }

        [Fact]
        public async Task GetCustomer_ReturnsOk_WhenCustomerExists()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();
            var customerId = Guid.NewGuid();
            var expectedCustomer = new CustomerDto
            {
                Id = customerId,
                CompanyName = "Contoso",
                ContactName = "Jane Doe",
                City = "Seattle"
            };

            mockService.Setup(s => s.GetCustomerById(customerId))
                       .ReturnsAsync(expectedCustomer);

            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.GetCustomer(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualCustomer = Assert.IsType<CustomerDto>(okResult.Value);
            Assert.Equal(expectedCustomer.Id, actualCustomer.Id);
            Assert.Equal(expectedCustomer.CompanyName, actualCustomer.CompanyName);
        }

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();
            var customerId = Guid.NewGuid();

            mockService.Setup(s => s.GetCustomerById(customerId))
                       .ReturnsAsync((CustomerDto?)null);

            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.GetCustomer(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateCustomer_ReturnsOk_WhenCustomerIsValid()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();
            var newCustomer = new CustomerDto
            {
                Id = Guid.NewGuid(),
                CompanyName = "Test Company",
                ContactName = "Jane Doe",
                Address = "123 Main St",
                City = "City",
                Region = "Region",
                PostalCode = "12345",
                Country = "USA",
                Phone = "123-456-7890"
            };

            mockService.Setup(s => s.CreateCustomer(newCustomer)).Returns(Task.CompletedTask);

            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.CreateCustomer(newCustomer);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.CreateCustomer(newCustomer), Times.Once);
        }

        [Fact]
        public async Task CreateCustomer_ReturnsBadRequest_WhenCustomerIsNull()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();
            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.CreateCustomer(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            mockService.Verify(s => s.CreateCustomer(It.IsAny<CustomerDto>()), Times.Never);
        }

        [Fact]
        public async Task EditCustomer_ReturnsOk_WhenEditIsSuccessful()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();
            var customerId = Guid.NewGuid();
            var updatedCustomer = new CustomerDto
            {
                Id = customerId,
                CompanyName = "Updated Co.",
                ContactName = "Jane Doe",
                Address = "123 Main St",
                City = "City",
                Region = "Region",
                PostalCode = "12345",
                Country = "USA",
                Phone = "123-456-7890"
            };

            mockService.Setup(s => s.EditCustomer(customerId, updatedCustomer))
                       .ReturnsAsync(true);

            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.EditCustomer(customerId, updatedCustomer);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.EditCustomer(customerId, updatedCustomer), Times.Once);
        }

        [Fact]
        public async Task EditCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();
            var customerId = Guid.NewGuid();
            var updatedCustomer = new CustomerDto
            {
                Id = customerId,
                CompanyName = "Updated Co."
            };

            mockService.Setup(s => s.EditCustomer(customerId, updatedCustomer))
                       .ReturnsAsync(false);

            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.EditCustomer(customerId, updatedCustomer);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockService.Verify(s => s.EditCustomer(customerId, updatedCustomer), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();
            var customerId = Guid.NewGuid();

            mockService.Setup(s => s.DeleteCustomer(customerId))
                       .ReturnsAsync(true);

            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.DeleteCustomer(customerId), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<ICustomerService>();
            var customerId = Guid.NewGuid();

            mockService.Setup(s => s.DeleteCustomer(customerId))
                       .ReturnsAsync(false);

            var controller = new CustomersController(mockService.Object);

            // Act
            var result = await controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockService.Verify(s => s.DeleteCustomer(customerId), Times.Once);
        }
    }
}