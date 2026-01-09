
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyApi.Controllers;
using Xunit;

public class ErrorHandlingControllerTests
{
    [Fact]
    public void Division_ReturnsBadRequest_WhenDenominatorIsZero()
    {
        var logger = new Mock<ILogger<ErrorHandlingController>>();
        var controller = new ErrorHandlingController(logger.Object);

        var result = controller.GetDivisionResult(10, 0) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void Division_ReturnsOk_WhenValid()
    {
        var logger = new Mock<ILogger<ErrorHandlingController>>();
        var controller = new ErrorHandlingController(logger.Object);

        var result = controller.GetDivisionResult(10, 2) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}
