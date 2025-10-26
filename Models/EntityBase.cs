using System;

namespace SampleLibraryMgmtSystem.Models
{
    public abstract class EntityBase
    {
        // Final model: non-nullable timestamps. Migrations were applied in two steps: add nullable then backfill,
        // now make them non-nullable to enforce presence.
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
