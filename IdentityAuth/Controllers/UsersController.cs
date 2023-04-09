using IdentityAuth.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityAuth.Controllers;

public class UsersController : CustomControllerBase
{
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("{userId:long}")]
    public async ValueTask<IActionResult> GetUserById([FromRoute] long userId)
    {
        var result = await _userManager.FindByIdAsync(userId.ToString());
        return result is not null ? Ok(result) : NotFound();
    }
}