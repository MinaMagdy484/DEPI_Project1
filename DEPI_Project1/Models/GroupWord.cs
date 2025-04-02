using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

[Index(nameof(Name), IsUnique = false)]
public class GroupWord
{
    [Key]
    public int ID { get; set; }
    [MaxLength(600)]
    public string Name { get; set; }
    public string? OriginLanguage { get; set; } // Nullable
    [MaxLength(200)]
    public string? GroupType { get; set; } // Nullable
    public string? EtymologyWord { get; set; } // Nullable
    public string? Etymology { get; set; } // Nullable

    public string? Notes { get; set; } // Nullable

    // Relationships
    [ValidateNever]
    public ICollection<Word> Words { get; set; }
    [ValidateNever]
    public ICollection<GroupExplanation> GroupExplanations { get; set; }
    [ValidateNever]
    public ICollection<GroupRelation> GroupParents { get; set; }

    [ValidateNever]
    public ICollection<GroupRelation> GroupChilds { get; set; }
}