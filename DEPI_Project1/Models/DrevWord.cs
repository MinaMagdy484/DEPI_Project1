using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


    public class DrevWord
    {
        [Key]
        public int ID { get; set; }

        // Self-Referencing Foreign Key
        public int WordID { get; set; }
        [ValidateNever]
        public Word Word1 { get; set; }

        public int RelatedWordID { get; set; }
        [ValidateNever]

        public Word Word2 { get; set; }
    }

