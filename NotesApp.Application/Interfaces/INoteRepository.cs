using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Application.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetNotesByUserAsync(int userId);
        Task<Note> GetNoteByIdAsync(int userId, int noteId);
        Task AddAsync(Note note);
        Task DeleteAsync(Note note);
        Task SaveChangesAsync();

    }
}
