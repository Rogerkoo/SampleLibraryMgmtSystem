namespace SampleLibraryMgmtSystem.Models;

public class Borrower : EntityBase
{
    public int Id { get; set; }  // Primary key
    public int MemberId { get; set; }
    public int BookId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsOverdue { get; set; }

    // Navigation properties
    public Books? Book { get; set; }
    public Members? Member { get; set; }
}