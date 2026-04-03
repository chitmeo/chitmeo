using BCrypt.Net;
using ChitMeo.Module.Auth.Application.UseCases.Auths.Commands;
using ChitMeo.Module.Auth.Application.UseCases.Tokens.Queries;
using ChitMeo.Module.Auth.Application.UseCases.Users.Commands;
using ChitMeo.Module.Auth.Application.Abstractions;
using ChitMeo.Module.Auth.Domain.Entities;
using ChitMeo.Module.Auth.Infrastructure.Persistence;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Security.Authentication;
using Microsoft.EntityFrameworkCore.InMemory;

namespace ChitMeo.Module.Auth.Tests;

public class AuthUseCasesTests : IDisposable
{
    private readonly AuthDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthUseCasesTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AuthDbContext(options);
        _dbContext.Database.EnsureCreated();
        _tokenService = Substitute.For<ITokenService>();
        _googleAuthService = Substitute.For<IGoogleAuthService>();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task PasswordLogin_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Name = "Test User", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var password = new UserPassword { Id = Guid.NewGuid(), UserId = user.Id, PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), PasswordSalt = "", IsActive = true };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.UserPasswords.AddAsync(password);
        await _dbContext.SaveChangesAsync();

        _tokenService.GenerateAccessToken(Arg.Any<User>()).Returns("access-token");
        _tokenService.HashToken(Arg.Any<string>()).Returns("hashed-token");

        var handler = new PasswordLogin.Handler(_dbContext, _tokenService);
        var command = new PasswordLogin.Command { Email = "test@example.com", Password = "password" };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().NotBeNull();
    }

    [Fact]
    public async Task PasswordLogin_InvalidEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange

        var handler = new PasswordLogin.Handler(_dbContext, _tokenService);
        var command = new PasswordLogin.Command { Email = "invalid@example.com", Password = "password" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task PasswordLogin_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Name = "Test User", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var password = new UserPassword { Id = Guid.NewGuid(), UserId = user.Id, PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"), PasswordSalt = "", IsActive = true };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.UserPasswords.AddAsync(password);
        await _dbContext.SaveChangesAsync();

        var handler = new PasswordLogin.Handler(_dbContext, _tokenService);
        var command = new PasswordLogin.Command { Email = "test@example.com", Password = "wrong" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task Register_ValidData_ReturnsUserId()
    {
        // Arrange
        var handler = new Register.Handler(_dbContext);
        var command = new Register.Command { Email = "new@example.com", Password = "password", Name = "New User" };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        var createdUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "new@example.com");
        createdUser.Should().NotBeNull();
        createdUser.Name.Should().Be("New User");
    }

    [Fact]
    public async Task Register_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingUser = new User { Id = Guid.NewGuid(), Email = "existing@example.com", Name = "Existing User", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        await _dbContext.Users.AddAsync(existingUser);
        await _dbContext.SaveChangesAsync();

        var handler = new Register.Handler(_dbContext);
        var command = new Register.Command { Email = "existing@example.com", Password = "password", Name = "User" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task GoogleLink_ValidToken_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "user@example.com", Name = "Test User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        _googleAuthService.ValidateAsync("valid-token")
            .Returns(Task.FromResult(new GoogleJsonWebSignature.Payload
            {
                Subject = "google-user-id",
                Email = "user@example.com"
            }));

        var handler = new GoogleLink.Handler(_dbContext, _googleAuthService);
        var command = new GoogleLink.Command { Token = "valid-token", UserId = userId };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        var externalLogin = await _dbContext.ExternalLogins.FirstOrDefaultAsync(el => el.UserId == userId && el.Provider == "Google");
        externalLogin.Should().NotBeNull();
    }

    [Fact]
    public async Task GoogleLink_InvalidToken_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "user@example.com", Name = "Test User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        _googleAuthService.ValidateAsync("invalid-token")
            .Returns<Task<GoogleJsonWebSignature.Payload>>(x => throw new InvalidJwtException("invalid"));

        var handler = new GoogleLink.Handler(_dbContext, _googleAuthService);
        var command = new GoogleLink.Command { Token = "invalid-token", UserId = userId };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GoogleUnlink_ExistingLink_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var externalLogin = new ExternalLogin { Id = Guid.NewGuid(), UserId = userId, Provider = "Google", ProviderUserId = "google-user-id", Email = "user@example.com", CreatedAt = DateTime.UtcNow };
        await _dbContext.ExternalLogins.AddAsync(externalLogin);
        await _dbContext.SaveChangesAsync();

        var handler = new GoogleUnlink.Handler(_dbContext);
        var command = new GoogleUnlink.Command { UserId = userId };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        var removed = await _dbContext.ExternalLogins.FirstOrDefaultAsync(el => el.UserId == userId && el.Provider == "Google");
        removed.Should().BeNull();
    }

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "user@example.com", Name = "Test User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var hashedOldToken = BCrypt.Net.BCrypt.HashPassword("old-token");
        var refreshToken = new RefreshToken { Id = Guid.NewGuid(), UserId = userId, Token = hashedOldToken, ExpiresAt = DateTime.UtcNow.AddDays(1), CreatedAt = DateTime.UtcNow, CreatedByIP = "127.0.0.1" };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        _tokenService.GenerateAccessToken(Arg.Any<User>()).Returns("new-access-token");
        _tokenService.HashToken(Arg.Any<string>()).Returns(x => x.Arg<string>() == "old-token" ? hashedOldToken : "hashed-new-token");

        var handler = new Refresh.Handler(_dbContext, _tokenService);
        var query = new Refresh.Query { RefreshToken = "old-token" };

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().NotBeNull();
    }

    [Fact]
    public async Task Logout_ValidUser_RemovesTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "user@example.com", Name = "Test User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var token1 = new RefreshToken { Id = Guid.NewGuid(), UserId = userId, Token = "token1", ExpiresAt = DateTime.UtcNow.AddDays(1), CreatedAt = DateTime.UtcNow, CreatedByIP = "127.0.0.1" };
        var token2 = new RefreshToken { Id = Guid.NewGuid(), UserId = userId, Token = "token2", ExpiresAt = DateTime.UtcNow.AddDays(1), CreatedAt = DateTime.UtcNow, CreatedByIP = "127.0.0.1" };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.RefreshTokens.AddAsync(token1);
        await _dbContext.RefreshTokens.AddAsync(token2);
        await _dbContext.SaveChangesAsync();

        var handler = new Logout.Handler(_dbContext);
        var command = new Logout.Command { UserId = userId };

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        var remainingTokens = await _dbContext.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
        remainingTokens.Should().BeEmpty();
    }
}