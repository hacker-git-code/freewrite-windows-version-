using System;

namespace FreewriteWindows.Models
{
    public class JournalEntry
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Date { get; set; } = string.Empty;
        public string PreviewText { get; set; } = string.Empty;
        public string Filename => $"[{Id}]-[{CreatedDate:yyyy-MM-dd-HH-mm-ss}].md";
    }
}