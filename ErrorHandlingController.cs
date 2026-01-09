
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorHandlingController : ControllerBase
    {
        private readonly ILogger<ErrorHandlingController> _logger;

        public ErrorHandlingController(ILogger<ErrorHandlingController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// يقسّم البسط على المقام ويعيد النتيجة. يمنع القسمة على صفر.
        /// مثال: GET /api/ErrorHandling/division?numerator=10&denominator=2
        /// </summary>
        [HttpGet("division")]
        public IActionResult GetDivisionResult([FromQuery] int numerator, [FromQuery] int denominator)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (denominator == 0)
            {
                _logger.LogWarning("Attempted division by zero: numerator={Numerator}", numerator);

                var problem = new ProblemDetails
                {
                    Title = "Invalid operation",
                    Detail = "Cannot divide by zero.",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                };

                return BadRequest(problem);
            }

            try
            {
                var result = numerator / denominator;
                _logger.LogInformation("Division performed: {Numerator} / {Denominator} = {Result}",
                    numerator, denominator, result);

                return Ok(new
                {
                    numerator,
                    denominator,
                    result
                });
            }
            catch (Exception ex)
            {
                // أي خطأ غير متوقع يُرفع للميدلوير العام
                _logger.LogError(ex, "Unexpected error during division.");
                throw;
            }
        }
    }
}
