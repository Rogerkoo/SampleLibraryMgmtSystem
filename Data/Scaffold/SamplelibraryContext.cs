using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SampleLibraryMgmtSystem.Models.Scaffold;

namespace SampleLibraryMgmtSystem.Data.Scaffold;

public partial class SamplelibraryContext : DbContext
{
    public SamplelibraryContext()
    {
    }

    public SamplelibraryContext(DbContextOptions<SamplelibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BooksEf> BooksEfs { get; set; }

    public virtual DbSet<BorrowersEf> BorrowersEfs { get; set; }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<MembersEf> MembersEfs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=samplelibrary.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BooksEf>(entity =>
        {
            entity.ToTable("BooksEF");
        });

        modelBuilder.Entity<BorrowersEf>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.BookId });

            entity.ToTable("BorrowersEF");
        });

        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<MembersEf>(entity =>
        {
            entity.HasKey(e => e.MemberId);

            entity.ToTable("MembersEF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
