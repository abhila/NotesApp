using NotesApp.Application.DTOs;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepo, ITokenService tokenService)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
        }

        public async Task<string> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username);

            if (user == null)
                throw new Exception("Invalid username or password");

            using var hmac = new HMACSHA256(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                throw new Exception("Invalid username or password");

            return _tokenService.GenerateToken(user);

        }

        public async Task<User> RegisterAsync(UserRegisterDto dto)
        {
            using var hmac = new HMACSHA256();

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return user;

        }
    }
}
