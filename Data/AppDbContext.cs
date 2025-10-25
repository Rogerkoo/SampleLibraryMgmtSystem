using System;
using Microsoft.EntityFrameworkCore;
using SampleLibraryMgmtSystem.Models;

namespace SampleLibraryMgmtSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    
    public DbSet<Books> BooksEF { get; set; }
    public DbSet<Borrower> BorrowersEF { get; set; }
    public DbSet<Members> MembersEF { get; set; }
}