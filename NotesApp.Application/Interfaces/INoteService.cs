using NotesApp.Application.DTOs;
using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Application.Interfaces
{
    public interface INoteService
    {
        Task<Note> CreateNoteAsync(int userId, NoteCreateDto dto);
        Task<Note> UpdateNoteAsync(int userId, int noteId, NoteUpdateDto dto);
        Task<bool> DeleteNoteAsync(int userId, int noteId);
        Task<IEnumerable<Note>> GetNotesAsync(int userId);
        Task<Note> GetNoteByIdAsync(int userId, int noteId);

    }
}
