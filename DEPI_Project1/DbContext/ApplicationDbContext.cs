using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DEPI_Project1.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseSqlServer("Server=localhost;Database=DEPI_Project2;Trusted_Connection = True;TrustServerCertificate=True;");
        optionsBuilder.UseSqlServer("Server=localhost;Database=Coptic(with login);Trusted_Connection = True;TrustServerCertificate=True;",
            options => options.CommandTimeout(120));

        base.OnConfiguring(optionsBuilder);
    }
     //public DbSet<UserType> UserTypes { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Admin> Admins { get; set; }

    public DbSet<Word> Words { get; set; }
    public DbSet<DictionaryReferenceWord> DictionaryReferenceWords { get; set; }
    public DbSet<Dictionary> Dictionaries { get; set; }
    public DbSet<GroupWord> Groups { get; set; }
    public DbSet<GroupExplanation> GroupExplanations { get; set; }
    public DbSet<GroupRelation> GroupRelations { get; set; } // Added GroupRelation DbSet

    public DbSet<WordExplanation> WordExplanations { get; set; }
    public DbSet<DrevWord> DrevWords { get; set; }
    public DbSet<WordMeaning> WordMeanings { get; set; }
    public DbSet<Meaning> Meanings { get; set; }
    public DbSet<Example> Examples { get; set; }
    public DbSet<WordMeaningBible> WordMeaningBibles { get; set; }
    public DbSet<Bible> Bibles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Data Seeding
        //new DbInitializer(modelBuilder).Seed();
        modelBuilder.Entity<IdentityRole>().HasData(
                   new IdentityRole()
                   {
                       Id = Guid.NewGuid().ToString(),
                       Name = "Admin",
                       NormalizedName = "Admin".ToUpper(),
                       ConcurrencyStamp = "Admin".ToUpper(),
                   },
                   new IdentityRole()
                   {
                        Id = Guid.NewGuid().ToString(),
                        Name = "User",
                        NormalizedName = "User".ToUpper(),
                        ConcurrencyStamp = "User".ToUpper(),
                   },
                    new IdentityRole()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Student",
                        NormalizedName = "Student".ToUpper(),
                        ConcurrencyStamp = "Student".ToUpper(),
                    },
                     new IdentityRole()
                     {
                         Id = Guid.NewGuid().ToString(),
                         Name = "Instructor",
                         NormalizedName = "Instructor".ToUpper(),
                         ConcurrencyStamp = "Instructor".ToUpper(),
                     }

        );
        #endregion

        modelBuilder.Ignore<Capture>();

        // Word to Group (One-to-Many)
        modelBuilder.Entity<Word>()
            .HasOne(w => w.GroupWord)
            .WithMany(g => g.Words)
            .HasForeignKey(w => w.GroupID);

        // Word to DictionaryReferenceWord (One-to-Many)
        modelBuilder.Entity<DictionaryReferenceWord>()
            .HasOne(drw => drw.Word)
            .WithMany(w => w.DictionaryReferenceWords)
            .HasForeignKey(drw => drw.WordID);

        // DictionaryReferenceWord to DictionaryReference (Many-to-One)
        modelBuilder.Entity<DictionaryReferenceWord>()
            .HasOne(drw => drw.Dictionary)
            .WithMany(dr => dr.DictionaryReferenceWords)
            .HasForeignKey(drw => drw.DictionaryID);

        // Group to GroupExplanation (One-to-Many)
        modelBuilder.Entity<GroupExplanation>()
            .HasOne(ge => ge.GroupWord)
            .WithMany(g => g.GroupExplanations)
            .HasForeignKey(ge => ge.GroupID);

        // Word to WordExplanation (One-to-Many)
        modelBuilder.Entity<WordExplanation>()
            .HasOne(we => we.Word)
            .WithMany(w => w.WordExplanations)
            .HasForeignKey(we => we.WordID);

        // Word to WordMeaning (One-to-Many)
        modelBuilder.Entity<WordMeaning>()
            .HasOne(wm => wm.Word)
            .WithMany(w => w.WordMeanings)
            .HasForeignKey(wm => wm.WordID);

        // WordMeaning to Meaning (One-to-One)
        modelBuilder.Entity<WordMeaning>()
            .HasOne(wm => wm.Meaning)
            .WithMany(m => m.WordMeanings)
            .HasForeignKey(wm => wm.MeaningID);

        // WordMeaning to Example (One-to-Many)
        modelBuilder.Entity<Example>()
            .HasOne(e => e.WordMeaning)
            .WithMany(wm => wm.Examples)
            .HasForeignKey(e => e.WordMeaningID);

        // WordMeaning to WordMeaningBible (One-to-Many)
        modelBuilder.Entity<WordMeaningBible>()
            .HasOne(wmb => wmb.WordMeaning)
            .WithMany(wm => wm.WordMeaningBibles)
            .HasForeignKey(wmb => wmb.WordMeaningID);

        modelBuilder.Entity<WordMeaningBible>()
            .HasOne(wmb => wmb.Bible)
            .WithMany(wm => wm.WordMeaningBibles)
            .HasForeignKey(wmb => wmb.BibleID);

        // Self-referencing relation for DrevWord (two Word IDs)
        modelBuilder.Entity<DrevWord>()
            .HasOne(dw => dw.Word1)
            .WithMany(w => w.DrevWords)
            .HasForeignKey(dw => dw.WordID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DrevWord>()
            .HasOne(dw => dw.Word2)
            .WithMany()
            .HasForeignKey(dw => dw.RelatedWordID)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing relation for Example (Parent-Child relationship)
        modelBuilder.Entity<Example>()
            .HasOne(e => e.ParentExample)
            .WithMany(e => e.ChildExamples)
            .HasForeignKey(e => e.ParentExampleID)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing relation for Meaning (Parent-Child relationship)
        modelBuilder.Entity<Meaning>()
            .HasOne(m => m.ParentMeaning)
            .WithMany(m => m.ChildMeanings)
            .HasForeignKey(m => m.ParentMeaningID)
            .OnDelete(DeleteBehavior.Restrict);

        // GroupRelation (Self-Referencing Relationship)
        modelBuilder.Entity<GroupRelation>()
            .HasOne(gr => gr.ParentGroup)
            .WithMany(g => g.GroupChilds)
            .HasForeignKey(gr => gr.ParentGroupID)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

        modelBuilder.Entity<GroupRelation>()
            .HasOne(gr => gr.RelatedGroup)
            .WithMany(g => g.GroupParents)
            .HasForeignKey(gr => gr.RelatedGroupID)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes




        

        base.OnModelCreating(modelBuilder);
    }

}
