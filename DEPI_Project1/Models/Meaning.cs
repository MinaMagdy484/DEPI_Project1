using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

[Index(nameof(MeaningText), IsUnique = false)]
public class Meaning
    {
        [Key]
        public int ID { get; set; }
    [MaxLength(600)]

    public string MeaningText { get; set; }
        public string? Notes { get; set; } // Nullable

        public string? Language { get; set; } // Nullable

    // Relationships
    [ValidateNever]
        public ICollection<WordMeaning> WordMeanings { get; set; }
        public int? ParentMeaningID { get; set; }
        [ValidateNever]
        public Meaning ParentMeaning { get; set; }
        [ValidateNever]
        public ICollection<Meaning> ChildMeanings { get; set; }
}
