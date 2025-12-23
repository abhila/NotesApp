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
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(Note note)
        {
            await _context.Notes.AddAsync(note);

        }

        public async Task DeleteAsync(Note note)
        {
            _context.Notes.Remove(note);

        }

        public async Task<Note> GetNoteByIdAsync(int userId, int noteId)
        {
            return await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

        }

        public async Task<IEnumerable<Note>> GetNotesByUserAsync(int userId)
        {
            return await _context.Notes
               .Where(n => n.UserId == userId)
               .ToListAsync();

        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

        }
    }
}
