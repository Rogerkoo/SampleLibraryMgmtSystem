using System.Text.Json.Serialization;

namespace SampleLibraryMgmtSystem.Models;

    public class Members : EntityBase
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsChild { get; set; }
        public bool IsActive { get; set; }

        [JsonConstructor]
        public Members(int memberId, string firstName, string surname, DateTime dateOfBirth, bool isChild, bool isActive)
        {
            MemberId = memberId;
            FirstName = firstName;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            IsChild = isChild;
            IsActive = isActive;
        }
            
        public Members(string firstName, string surname, DateTime dateOfBirth, bool isChild, bool isActive)
        {
            FirstName = firstName;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            IsChild = isChild;
            IsActive = isActive;
        }        
    }

