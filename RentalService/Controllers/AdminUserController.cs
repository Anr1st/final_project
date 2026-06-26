using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalService.Models;

namespace RentalService.Controllers;

[ApiController]
public class AdminUsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminUsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("admin/users")]
    [HttpGet("api/v1/admin/users")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminUserResponse(u.Id, u.Email, u.FullName, u.Role, u.IsVerified, u.IsBlocked, u.CreatedAt))
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("admin/users/pending")]
    [HttpGet("api/v1/admin/users/pending")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var users = await _db.Users
            .Where(u => !u.IsVerified && !u.IsBlocked)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminUserResponse(u.Id, u.Email, u.FullName, u.Role, u.IsVerified, u.IsBlocked, u.CreatedAt))
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("admin/users/{id:guid}/verify")]
    [HttpPut("admin/users/{id:guid}/verify")]
    [HttpPost("api/v1/admin/users/{id:guid}/verify")]
    [HttpPut("api/v1/admin/users/{id:guid}/verify")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> VerifyUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsVerified = true;
        await _db.SaveChangesAsync();

        return Ok(ToResponse(user));
    }

    [HttpPost("admin/users/{id:guid}/block")]
    [HttpPut("admin/users/{id:guid}/block")]
    [HttpPost("api/v1/admin/users/{id:guid}/block")]
    [HttpPut("api/v1/admin/users/{id:guid}/block")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> BlockUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsBlocked = true;
        await _db.SaveChangesAsync();

        return Ok(ToResponse(user));
    }

    private static AdminUserResponse ToResponse(User user)
    {
        return new AdminUserResponse(user.Id, user.Email, user.FullName, user.Role, user.IsVerified, user.IsBlocked, user.CreatedAt);
    }

    public record AdminUserResponse(Guid Id, string Email, string FullName, UserRole Role, bool IsVerified, bool IsBlocked, DateTime CreatedAt);
}