using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


public class WordExplanation
    {
        [Key]
        public int ID { get; set; }
        public string Explanation { get; set; }
        public string Language { get; set; }
        public string? Notes { get; set; } // Nullable

        // Foreign Key
        public int WordID { get; set; }
        [ValidateNever]
        public Word Word { get; set; }

    }

