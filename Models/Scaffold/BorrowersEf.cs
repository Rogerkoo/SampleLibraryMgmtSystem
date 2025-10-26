using System;
using System.Collections.Generic;

namespace SampleLibraryMgmtSystem.Models.Scaffold;

public partial class BorrowersEf
{
    public int MemberId { get; set; }

    public int BookId { get; set; }

    public string StartDate { get; set; } = null!;

    public string DueDate { get; set; } = null!;

    public string ReturnDate { get; set; } = null!;
}
