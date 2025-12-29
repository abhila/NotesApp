using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NotesApp.Application.DTOs;
using NotesApp.Application.Interfaces;
using NotesApp.Application.Services;
using NotesApp.Domain.Entities;
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

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_HashPassword_AndCallRepository()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            var mockToken = new Mock<ITokenService>();

            // Simulate repository assigning an Id
            mockRepo
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => u.Id = 11)
                .Returns(Task.CompletedTask);

            mockRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new UserService(mockRepo.Object, mockToken.Object);

            var dto = new UserRegisterDto
            {
                Username = "alice",
                Email = "alice@example.com",
                Password = "P@ssw0rd!"
            };

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(11, result.Id);
            Assert.Equal(dto.Username, result.Username);
            Assert.Equal(dto.Email, result.Email);
            Assert.NotNull(result.PasswordHash);
            Assert.NotNull(result.PasswordSalt);
            Assert.True(result.PasswordHash.Length > 0);
            Assert.True(result.PasswordSalt.Length > 0);

            // Verify stored hash matches recomputed hash using stored salt
            using var hmac = new HMACSHA256(result.PasswordSalt);
            var expectedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            Assert.True(expectedHash.SequenceEqual(result.PasswordHash));

            mockRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.Username == dto.Username && u.Email == dto.Email)), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
