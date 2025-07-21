using Xunit;
using BookStoreAPI.Services;
using BookStoreAPI.Data;
using BookStoreAPI.DTOs;
using BookStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class AuthServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"BookStoreTestDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    // Fake TokenService to bypass actual JWT creation
    public class FakeTokenService : TokenService
    {
        public FakeTokenService() : base(null) { }

        public override string CreateToken(User user)
        {
            return "fake-jwt-token";
        }
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnToken_WhenUserIsNew()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var tokenService = new FakeTokenService();
        var authService = new AuthService(context, tokenService);

        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "P@ssw0rd"
        };

        // Act
        var token = await authService.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(token);
        Assert.Equal("fake-jwt-token", token);
    }
}
