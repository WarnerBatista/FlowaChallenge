using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderGenerator.Api.Controllers;
using OrderGenerator.Api.Dtos;
using OrderGenerator.Api.Services.Interfaces;

namespace OrderGenerator.UnitTests.Controllers;

public class OrdersControllerTests
{
    [Fact]
    public void Constructor_Throws_When_FixService_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new OrdersController(null!));
    }

    [Fact]
    public void SendOrder_ReturnsAcceptedResult_WithExpectedResponse()
    {
        // Arrange
        var order = new OrderRequest
        (
            "AAPL",
            "Buy",
            100,
            150.00m
        );
        var fixInitiatorService = Substitute.For<IFixInitiatorService>();
        var expectedResponse = new OrderResponse
            (
                "Order received",
                "123",
                "Pending"
            );

        fixInitiatorService.SendOrder(order).Returns(expectedResponse);
        var sut = new OrdersController(fixInitiatorService);

        // Act
        var result = sut.SendOrder(order);

        // Assert
        Assert.IsType<AcceptedResult>(result);
        Assert.NotNull(result);
        var acceptedResult = (AcceptedResult)result;
        Assert.Equal(202, acceptedResult.StatusCode);
        Assert.Equal(expectedResponse, acceptedResult.Value);
    }
}
