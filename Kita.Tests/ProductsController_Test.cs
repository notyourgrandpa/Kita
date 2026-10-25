using Kita.Application.DTOs;
using Kita.Application.Products.Queries;
using Kita.Application.Services;
using Kita.Domain.Categories;
using Kita.Domain.Products;
using Kita.Infrastructure.Persistence;
using Kita.Server.Controllers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Kita.Tests;

public class ProductsController_Test
{
    [Fact]
    public async Task GetProduct()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        using var context = new ApplicationDbContext(options);
        context.Add(new Product
        {
            Id = 1,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Test category" },
            CostPrice = 1,
            SellingPrice = 1,
            IsActive = true,
            ProductCode = "Product-123",
            Name = "TestProduct1"
        });
        context.SaveChanges();

        var handler = new GetProductHandler(context);
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<GetProductQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetProductQuery query, CancellationToken token) => handler.Handle(query, token));
        var controller = new ProductsController(Mock.Of<IProductService>(), mediator.Object);

        var result = await controller.GetProductAsync(1);
        var product = Assert.IsType<ProductDTO>(result.Value);
        Assert.Equal(1, product.Id);
        Assert.Equal("TestProduct1", product.Name);
        Assert.Equal("Product-123", product.ProductCode);
        Assert.Equal("Test category", product.CategoryName);

        // Missing products are mapped to HTTP responses by the exception middleware.
        await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.GetProductAsync(2));
    }
}
