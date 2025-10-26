using System;
using System.Collections.Generic;

namespace SampleLibraryMgmtSystem.Models.Scaffold;

public partial class MembersEf
{
    public int MemberId { get; set; }

    public string FirstName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string DateOfBirth { get; set; } = null!;

    public int IsChild { get; set; }

    public int IsActive { get; set; }
}
