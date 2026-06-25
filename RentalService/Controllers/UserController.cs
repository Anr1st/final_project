using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalService.Models;
using RentalService.Services;

namespace RentalService.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly MockVerificationService _verificationService;

    public UsersController(AppDbContext db, MockVerificationService verificationService)
    {
        _db = db;
        _verificationService = verificationService;
    }

    [HttpGet("profile")]
    [HttpGet("api/v1/users/profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(ToResponse(user));
    }

    [HttpPut("profile")]
    [HttpPut("api/v1/users/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            return Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var emailExists = await _db.Users.AnyAsync(u => u.Email == email && u.Id != user.Id);
            if (emailExists)
            {
                return Conflict("этот email занят другим пользователем");
            }

            user.Email = email;
        }

        var newName = request.Name ?? request.FullName;
        if (!string.IsNullOrWhiteSpace(newName))
        {
            user.FullName = newName.Trim();
        }

        await _db.SaveChangesAsync();
        return Ok(ToResponse(user));
    }

    [HttpPost("verify-identity")]
    [HttpPost("api/v1/users/verify-identity")]
    [Authorize]
    public async Task<IActionResult> VerifyIdentity()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            return Unauthorized();
        }

        var verified = _verificationService.VerifyIdentity(user);
        if (!verified)
        {
            return BadRequest("ошибка верификации");
        }

        user.IsVerified = true;
        await _db.SaveChangesAsync();

        return Ok(ToResponse(user));
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsBlocked);
    }

    private static UserResponse ToResponse(User user)
    {
        return new UserResponse(user.Id, user.Email, user.FullName, user.Role, user.IsVerified, user.IsBlocked, user.CreatedAt);
    }

    public class UpdateProfileRequest
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? FullName { get; set; }
    }

    public record UserResponse(Guid Id, string Email, string FullName, UserRole Role, bool IsVerified, bool IsBlocked, DateTime CreatedAt);
}