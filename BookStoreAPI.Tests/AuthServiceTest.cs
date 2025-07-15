using Xunit;
using Microsoft.EntityFrameworkCore;
using BookStoreAPI.Data;
using BookStoreAPI.Services;
using BookStoreAPI.DTOs;
using BookStoreAPI.Models;
using Microsoft.Extensions.Configuration;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly ApplicationDbContext _context;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _context = new ApplicationDbContext(options);

        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                { "Jwt:Key", "supersecretkey_very_long_256bit_key!" },
                { "Jwt:Audience", "BookStoreClient" }
            }).Build();
        var config = configurationRoot;

        var tokenService = new TokenService(config);
        _authService = new AuthService(_context, tokenService);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@test.com",
            Password = "testpass"
        };

        var token = await _authService.RegisterAsync(dto);

        Assert.NotNull(token);
        Assert.True(await _context.Users.AnyAsync(u => u.Username == "testuser"));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange: seed user manually
        var password = "testpass";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        _context.Users.Add(new User
        {
            Username = "loginuser",
            Email = "login@test.com",
            PasswordHash = hashedPassword
        });
        await _context.SaveChangesAsync();

        var dto = new LoginDto
        {
            Username = "loginuser",
            Password = password
        };

        // Act
        var token = await _authService.LoginAsync(dto);

        // Assert
        Assert.NotNull(token);
        Assert.True(token.Length > 20); // basic validation
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenUserAlreadyExists()
    {
        // Arrange: seed user
        _context.Users.Add(new User
        {
            Username = "duplicateuser",
            Email = "existing@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("any")
        });
        await _context.SaveChangesAsync();

        var dto = new RegisterDto
        {
            Username = "duplicateuser",
            Email = "newuser@test.com",
            Password = "newpass"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _authService.RegisterAsync(dto)
        );

        Assert.Equal("Username already taken", ex.Message);
    }


}
