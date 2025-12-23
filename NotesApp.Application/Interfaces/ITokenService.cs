using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);

    }
}
