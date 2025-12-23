using Microsoft.EntityFrameworkCore;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using NotesApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);

        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);

        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

        }
    }
}
