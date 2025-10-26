using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SampleLibraryMgmtSystem.Models;

namespace SampleLibraryMgmtSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Borrower>().HasKey(table => table.Id);
        builder.Entity<Members>().HasKey(table => table.MemberId);
    }
    
    // Set timestamps automatically on save
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is SampleLibraryMgmtSystem.Models.EntityBase && (e.State == EntityState.Added || e.State == EntityState.Modified)))
        {
            var entity = (SampleLibraryMgmtSystem.Models.EntityBase)entry.Entity;
            if (entry.State == EntityState.Added)
            {
                entity.DateAdded = utcNow;
                entity.DateUpdated = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.DateUpdated = utcNow;
            }
        }
    }
    
    public void InsertIntialData()
    {
        // Initial data seeding can be done here if needed
        if (!this.BooksEF.Any())
        {
            this.BooksEF.AddRange(
                new Books(1, "To Kill a Mockingbird", "Harper Lee", "Fiction", "9780446310789", "Grand Central Publishing", new DateTime(1925, 4, 10), 5),
                new Books(2, "Hercule Poirot: The Complete Short Stories", "Agatha Christie", "Fiction", "9780006513773", "HarperCollins", new DateTime(1999, 1, 2), 2),
                new Books(3, "The Shipping News", "Annie Proulx", "Fiction", "0743225406", "Scribner", new DateTime(1993, 1, 2), 3),
                new Books(4, "Sons of Fortune", "Jeffrey Archer", "Fiction", "9781447221838", "Pan Books", new DateTime(2002, 2, 11), 2),
                new Books(5, "Bleak House", "Charles Dickens", "Fiction", "9780140621204", "Penguin Classics", new DateTime(1994, 1, 3), 10)
            );
            this.SaveChanges();
            // Ensure seeded records have the requested DateAdded/DateUpdated value
            var backfillDate = new DateTime(2025, 10, 26, 0, 0, 0, DateTimeKind.Utc);
            foreach (var b in this.BooksEF)
            {
                if (b.DateAdded == default) b.DateAdded = backfillDate;
                if (b.DateUpdated == default) b.DateUpdated = backfillDate;
            }
            this.SaveChanges();
        }
        if (!this.MembersEF.Any())
        {
            this.MembersEF.AddRange(
                new Members(1, "Teng Joon", "Koo", new DateTime(1985, 10, 11), false, true),
                new Members(2, "Jane", "Lee", new DateTime(1988, 9, 12), false, true),
                new Members(3, "Rachel", "Lim", new DateTime(2020, 12, 3), true, true),
                new Members(4, "Luke", "Wong", new DateTime(2021, 3, 30), true, true),
                new Members(5, "Wei Yee", "Loh", new DateTime(1978, 6, 30), false, false)
            );
            this.SaveChanges();
            var backfillDate = new DateTime(2025, 10, 26, 0, 0, 0, DateTimeKind.Utc);
            foreach (var m in this.MembersEF)
            {
                if (m.DateAdded == default) m.DateAdded = backfillDate;
                if (m.DateUpdated == default) m.DateUpdated = backfillDate;
            }
            this.SaveChanges();
        }
    }
    
    public DbSet<Books> BooksEF { get; set; }
    public DbSet<Borrower> BorrowersEF { get; set; }
    public DbSet<Members> MembersEF { get; set; }

}