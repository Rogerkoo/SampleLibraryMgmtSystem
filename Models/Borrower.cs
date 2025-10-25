namespace SampleLibraryMgmtSystem.Models;

public class Borrower
{
    public int MemberId { get; set; }

    public int BookId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ReturnDate { get; set; }
}