using Microsoft.AspNetCore.Mvc;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("demo-login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult DemoLogin([FromBody] DemoLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return ValidationProblem();
        }

        return Ok(new DemoLoginResponse(
            "demo-user",
            request.Username,
            "crm.read crm.write analytics.read",
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.demo-token"));
    }

    public sealed record DemoLoginRequest(string Username, string Password);
    public sealed record DemoLoginResponse(string UserId, string Username, string Roles, string AccessToken);
}
