using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.Application.DTOs
{
    public class NoteUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

    }
}
