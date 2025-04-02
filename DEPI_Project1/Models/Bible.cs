using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
[Index(nameof(Book), nameof(Chapter), nameof(Verse), nameof(Language), nameof(Edition))]
[Index(nameof(Text), IsUnique = false)]
public class Bible
    {
        [Key]
        public int BibleID { get; set; }
        public int Book { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public string Language { get; set; }
        public string Edition { get; set; }
        public string Text { get; set; }
        public string? Pronunciation { get; set; } // Nullable
        public string? Notes { get; set; } // Nullable

        // Relationships
        [ValidateNever]
        public ICollection<WordMeaningBible> WordMeaningBibles { get; set; }
    }

//public class BibleVerse
//{
//    public int BibleVerseID { get; set; }
//    public string Book { get; set; }
//    public int Chapter { get; set; }
//    public int Verse { get; set; }
//    public string Edition { get; set; }
//    public string Language { get; set; }
//    public string Text { get; set; }
//    public string Pronunciation { get; set; }

//    public ICollection<WordMeaningBible> WordMeaningBibles { get; set; }
//}