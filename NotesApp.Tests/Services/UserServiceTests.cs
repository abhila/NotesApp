using Moq;
using NotesApp.Application.DTOs;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Xunit;

namespace NotesApp.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            var mockTokenService = new Mock<ITokenService>();

            // Prepare a valid password and compute matching hash/salt using HMACSHA256,
            // so the UserService's HMAC-based check will succeed.
            var plainPassword = "password123";
            byte[] passwordSalt;
            byte[] passwordHash;
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
            }

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            // Simulate repository returning a user
            mockRepo.Setup(r => r.GetByUsernameAsync("testuser"))
                    .ReturnsAsync(user);

            // Simulate token service returning a fake JWT
            mockTokenService.Setup(t => t.GenerateToken(user))
                            .Returns("fake-jwt-token");

            var service = new UserService(mockRepo.Object, mockTokenService.Object);

            // Act
            var loginDto = new UserLoginDto { Username = "testuser", Password = plainPassword };
            var result = await service.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("fake-jwt-token", result);
            mockTokenService.Verify(t => t.GenerateToken(user), Times.Once);
        }

    }
}
