using ClaimsManagement.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace ClaimsManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    [HttpGet("[action]")]
    public async ValueTask<IActionResult> Me()
    {
        var claimsTest = HttpContext.User.Claims;
        var client = new GitHubClient(new ProductHeaderValue("test"))
        {
            Credentials = new Credentials(HttpContext.User.GetAccessToken())
        };
        var data = await client.User.Get(User.Identity?.Name);
        return data != null ? Ok(data) : BadRequest();
    }
}