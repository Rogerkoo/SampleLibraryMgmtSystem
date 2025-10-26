using System;
using System.Collections.Generic;

namespace SampleLibraryMgmtSystem.Models.Scaffold;

public partial class BooksEf
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public string DatePublished { get; set; } = null!;

    public int AvailableCopies { get; set; }
}
