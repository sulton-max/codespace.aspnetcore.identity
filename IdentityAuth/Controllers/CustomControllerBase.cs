using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomControllerBase : ControllerBase
{
}