using System.Security.Claims;
using ChoreQuest.Api.DTOs;
using ChoreQuest.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChoreQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.RegisterAsync(request);

        if (response == null)
        {
            return BadRequest(new { message = "Username or email already exists" });
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var profile = await _authService.GetUserProfileAsync(userId);

        if (profile == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(profile);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var profile = await _authService.UpdateUserProfileAsync(userId, request);

        if (profile == null)
        {
            return BadRequest(new { message = "Unable to update profile. Email may already be taken." });
        }

        return Ok(profile);
    }
}
