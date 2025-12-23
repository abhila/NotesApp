using NotesApp.Application.DTOs;
using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(UserRegisterDto dto);
        Task<string> LoginAsync(UserLoginDto dto);

    }
}
