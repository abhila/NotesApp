using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task SaveChangesAsync();

    }
}
