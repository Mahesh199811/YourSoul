using SQLite;
using System.Collections.ObjectModel;

namespace YourSoulApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Username { get; set; }
        public string Password { get; set; } // In a real app, this should be hashed
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } // "Male", "Female", "Other"
        public string Bio { get; set; }
        public string ProfileImagePath { get; set; }
        public string Location { get; set; }
        
        // Dating preferences
        public string InterestedIn { get; set; } // "Male", "Female", "Both"
        public int MinAgePreference { get; set; } = 18;
        public int MaxAgePreference { get; set; } = 100;
        public int MaxDistance { get; set; } = 50; // in kilometers
        
        // These properties are not stored in the database but used for UI
        [Ignore]
        public ObservableCollection<string> Photos { get; set; } = new ObservableCollection<string>();
        
        [Ignore]
        public bool IsCurrentUser { get; set; }
    }
}
