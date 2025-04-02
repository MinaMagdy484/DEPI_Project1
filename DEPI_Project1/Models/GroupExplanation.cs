using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


public class GroupExplanation
    {
        [Key]
        public int ID { get; set; }
        public string Explanation { get; set; }
        public string Language { get; set; }
        public string? Notes { get; set; } // Nullable

        // Foreign Key
        public int GroupID { get; set; }
        [ValidateNever]
        public GroupWord GroupWord { get; set; }
    }
