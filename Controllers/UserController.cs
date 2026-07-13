using Microsoft.AspNetCore.Mvc;
using PassXYZ.Server.DTOs.User;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var profile = await _userService.GetUserProfile(username);
        if (profile == null) return NotFound();

        return Ok(profile);
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var email = string.IsNullOrEmpty(request.Email) 
            ? $"{request.Username}@localhost" 
            : request.Email;

        var result = await _userService.CreateUser(request, email);
        if (!result)
        {
            return Conflict("User already exists");
        }

        var profile = await _userService.GetUserProfile(request.Username);
        return Ok(profile);
    }

    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var currentUser = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(currentUser) || currentUser != username)
        {
            return Unauthorized();
        }

        var result = await _userService.DeleteUser(username);
        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await _userService.Login(request, request.Email, ipAddress);

        if (!result.Success)
        {
            if (result.ConflictSession != null)
            {
                return Conflict(new SessionConflictResponse
                {
                    Message = "Another device is logged in",
                    ExistingSession = new ExistingSession
                    {
                        DeviceInfo = result.ConflictSession.DeviceInfo,
                        LoginTime = result.ConflictSession.LoginTime,
                        IpAddress = result.ConflictSession.IpAddress
                    },
                    ConfirmUrl = $"/api/user/login?takeOver=true"
                });
            }
            return Unauthorized();
        }

        var profile = await _userService.GetUserProfile(result.Username ?? string.Empty);

        return Ok(new LoginResponse
        {
            Token = result.Token ?? string.Empty,
            ExpiresAt = result.ExpiresAt ?? DateTime.Now,
            User = profile
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        await _userService.Logout(username);
        return Ok();
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var result = await _userService.UpdateUserProfile(username, request);
        if (!result) return NotFound();

        return Ok();
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetUsersList()
    {
        var users = await _userService.GetUsersList();
        return Ok(users);
    }

    [HttpGet("by-email")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        var profile = await _userService.GetUserByEmail(email);
        if (profile == null) return NotFound();
        return Ok(profile);
    }
}