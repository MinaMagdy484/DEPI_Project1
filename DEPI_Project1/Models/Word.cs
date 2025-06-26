using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
[Index(nameof(Word_text), IsUnique = false)]
public class Word
    {
        [Key]
        public int WordId { get; set; }
        [Required]
    [MaxLength(500)]
    public string Word_text { get; set; }
        [Required]
        public string? Language { get; set; }
        [Required]
        public string? Class { get; set; }

        public string? notes { get; set; }

        public string? IPA { get; set; } // Nullable
        public string? Pronunciation { get; set; } // Nullable
        public bool IsDrevWord { get; set; }
        public bool  IsReviewed { get; set; }
        public bool? Review1 { get; set; }
        public bool? Review2 { get; set; }
            
        public int? RootID { get; set; }
        [ValidateNever]
        public Word Root { get; set; }

        public int? GroupID { get; set; }
        [ValidateNever]
        public GroupWord GroupWord { get; set; }
        [ValidateNever]
        public ICollection<WordMeaning> WordMeanings { get; set; }
        [ValidateNever]
        public ICollection<DictionaryReferenceWord> DictionaryReferenceWords { get; set; }
        [ValidateNever]
        public ICollection<WordExplanation> WordExplanations { get; set; }
        [ValidateNever]
        public ICollection<DrevWord> DrevWords { get; set; }
    }
