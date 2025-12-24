using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Application.DTOs;
using NotesApp.Application.Interfaces;

namespace NotesApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints with JWT

    public class NoteController : Controller
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        // GET: api/note
        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            var userId = GetUserId();
            var notes = await _noteService.GetNotesAsync(userId);
            return Ok(notes);
        }

        // GET: api/note/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteById(int id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetNoteByIdAsync(userId, id);
            if (note == null) return NotFound();
            return Ok(note);
        }

        // POST: api/note
        [HttpPost]
        public async Task<IActionResult> Create(NoteCreateDto dto)
        {
            var userId = GetUserId();
            var note = await _noteService.CreateNoteAsync(userId, dto);
            return Ok(note);
        }

        // PUT: api/note/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NoteUpdateDto dto)
        {
            var userId = GetUserId();
            var note = await _noteService.UpdateNoteAsync(userId, id, dto);
            if (note == null) return NotFound();
            return Ok(note);
        }

        // DELETE: api/note/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _noteService.DeleteNoteAsync(userId, id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // Helper method to extract userId from JWT claims
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        }


    }
}
