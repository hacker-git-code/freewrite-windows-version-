using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FreewriteWindows.Models;

namespace FreewriteWindows.Services
{
    public class FileService
    {
        private readonly string documentsPath;

        public FileService()
        {
            var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            documentsPath = Path.Combine(myDocuments, "Freewrite");
            
            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }
        }

        public void SaveEntry(JournalEntry entry, string content)
        {
            var filePath = Path.Combine(documentsPath, entry.Filename);
            File.WriteAllText(filePath, content);
        }

        public string LoadEntry(JournalEntry entry)
        {
            var filePath = Path.Combine(documentsPath, entry.Filename);
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return string.Empty;
        }

        public List<JournalEntry> LoadAllEntries()
        {
            var entries = new List<JournalEntry>();
            
            if (!Directory.Exists(documentsPath))
            {
                return entries;
            }

            var files = Directory.GetFiles(documentsPath, "*.md");
            
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                
                // Pattern: [uuid]-[yyyy-MM-dd-HH-mm-ss].md
                var match = Regex.Match(filename, @"\[(.*?)\]-\[(\d{4}-\d{2}-\d{2}-\d{2}-\d{2}-\d{2})\]\.md");
                
                if (match.Success)
                {
                    if (Guid.TryParse(match.Groups[1].Value, out Guid id))
                    {
                        if (DateTime.TryParseExact(match.Groups[2].Value, 
                            "yyyy-MM-dd-HH-mm-ss", 
                            null, 
                            System.Globalization.DateTimeStyles.None, 
                            out DateTime createdDate))
                        {
                            var content = File.ReadAllText(file);
                            var preview = content
                                .Replace("\n", " ")
                                .Replace("\r", "")
                                .Trim();
                            
                            var truncated = preview.Length > 30 
                                ? preview.Substring(0, 30) + "..." 
                                : preview;

                            entries.Add(new JournalEntry
                            {
                                Id = id,
                                CreatedDate = createdDate,
                                Date = createdDate.ToString("MMM d"),
                                PreviewText = truncated
                            });
                        }
                    }
                }
            }
            
            return entries;
        }

        public void DeleteEntry(JournalEntry entry)
        {
            var filePath = Path.Combine(documentsPath, entry.Filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void ExportEntry(JournalEntry entry, string exportPath)
        {
            var filePath = Path.Combine(documentsPath, entry.Filename);
            if (File.Exists(filePath))
            {
                File.Copy(filePath, exportPath, true);
            }
        }
    }
}