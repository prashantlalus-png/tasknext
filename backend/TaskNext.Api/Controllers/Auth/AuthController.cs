using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskNext.Api.Data;
using TaskNext.Api.Dtos;
using TaskNext.Api.Models;
using TaskNext.Api.Services;
using BCrypt.Net;

namespace TaskNext.Api.Controllers; // Nest ko Next kar diya

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokens;

    public AuthController(AppDbContext db, ITokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email == email);
        if (exists) return BadRequest("Email already registered.");

        var user = new User
        {
            Name = req.Name.Trim(),
            Email = email,
            // Password ko hash karna zaroori hai
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var (token, expiresAtUtc) = _tokens.CreateToken(user);
        return Ok(new AuthResponse(token, expiresAtUtc, user.Id, user.Name, user.Email));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return Unauthorized("Invalid credentials.");

        var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!ok) return Unauthorized("Invalid credentials.");

        var (token, expiresAtUtc) = _tokens.CreateToken(user);
        return Ok(new AuthResponse(token, expiresAtUtc, user.Id, user.Name, user.Email));
    }
}