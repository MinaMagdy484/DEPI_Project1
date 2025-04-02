using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class Example
    {
        [Key]
        public int ID { get; set; }
        public string ExampleText { get; set; }
        public string? Reference { get; set; }
        public string? Pronunciation { get; set; } // Nullable
        public string? Notes { get; set; } // Nullable
        public string? Language { get; set; } // Nullable

    // Foreign Key
    public int? WordMeaningID { get; set; }
        [ValidateNever]
        public WordMeaning WordMeaning { get; set; }
        public int? ParentExampleID { get; set; }
        [ValidateNever]
        public Example ParentExample { get; set; }
        [ValidateNever]
        public ICollection<Example> ChildExamples { get; set; }


    }
