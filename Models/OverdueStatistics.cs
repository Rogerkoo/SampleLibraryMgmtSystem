using System;

namespace SampleLibraryMgmtSystem.Models
{
    public class OverdueStatistics
    {
        public int TotalOverdueBooks { get; set; }
        public int TotalActiveOverdueBooks { get; set; }
        public decimal AverageOverdueDays { get; set; }
        public Dictionary<string, int> OverdueByGenre { get; set; } = new();
        public List<TopOverdueItem> MostOverdueBooks { get; set; } = new();
        public List<TopOverdueItem> MostFrequentlyOverdueMembers { get; set; } = new();
    }

    public class TopOverdueItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public int TotalDaysOverdue { get; set; }
    }
}