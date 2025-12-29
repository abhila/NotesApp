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
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<Note> CreateNoteAsync(int userId, NoteCreateDto dto)
        {
            var now = DateTime.UtcNow;

            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = userId,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _noteRepository.AddAsync(note);
            await _noteRepository.SaveChangesAsync();

            return note;
        }

        public async Task<bool> DeleteNoteAsync(int userId, int noteId)
        {
            var note = await _noteRepository.GetNoteByIdAsync(userId, noteId);
            if (note == null)
                return false;

            await _noteRepository.DeleteAsync(note);
            await _noteRepository.SaveChangesAsync();
            return true;
        }

        public async Task<Note> GetNoteByIdAsync(int userId, int noteId)
        {
            var note = await _noteRepository.GetNoteByIdAsync(userId, noteId);
            return note;
        }

        public async Task<IEnumerable<Note>> GetNotesAsync(int userId)
        {
            return await _noteRepository.GetNotesByUserAsync(userId);
        }

        public async Task<Note> UpdateNoteAsync(int userId, int noteId, NoteUpdateDto dto)
        {
            var note = await _noteRepository.GetNoteByIdAsync(userId, noteId);
            if (note == null)
                return null;

            // Update only when values are provided (non-empty); adjust as desired for different semantics.
            if (!string.IsNullOrWhiteSpace(dto.Title))
                note.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Content))
                note.Content = dto.Content;

            note.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.SaveChangesAsync();
            return note;
        }
    }
}
