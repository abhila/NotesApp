using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using NotesApp.Domain.Entities;
using NotesApp.Infrastructure.Services;
using System.Collections.Generic;
using Xunit;

public class TokenServiceTests
{
    [Fact]
    public void GenerateToken_ShouldReturnValidJwtString()
    {
        // Arrange: fake configuration with JWT settings
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "ThisIsASuperSecretKeyForTesting1234567890!" }, // 40+ chars
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpiresInMinutes", "60" }
        };


        Microsoft.Extensions.Configuration.IConfiguration configuration =
    new Microsoft.Extensions.Configuration.ConfigurationBuilder()
        .AddInMemoryCollection(inMemorySettings)
        .Build();


        var tokenService = new TokenService(configuration);

        var user = new User
        {
            Id = 1,
            Username = "testuser"
        };

        // Act
        var token = tokenService.GenerateToken(user);

        // Assert
        Assert.False(string.IsNullOrEmpty(token)); // token should not be empty
        Assert.Contains("eyJ", token); // JWTs always start with "eyJ"
    }
}