using BookStoreAPI.Data;
using BookStoreAPI.DTOs;
using BookStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly TokenService _tokenService;

    public AuthService(ApplicationDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new Exception("Username already taken");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Email already registered");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _tokenService.CreateToken(user);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        User? user;

        if (dto.Identifier.Contains("@"))
        {
            // Login using email
            user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Identifier);
        }
        else
        {
            // Login using username
            user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Identifier);
        }

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid username or password");

        return _tokenService.CreateToken(user);
    }
}
