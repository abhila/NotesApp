using NotesApp.Application.DTOs;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Application.Services
{
    public class NoteService : INoteService
    {
        public Task<Note> CreateNoteAsync(int userId, NoteCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteNoteAsync(int userId, int noteId)
        {
            throw new NotImplementedException();
        }

        public Task<Note> GetNoteByIdAsync(int userId, int noteId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Note>> GetNotesAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Note> UpdateNoteAsync(int userId, int noteId, NoteUpdateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
