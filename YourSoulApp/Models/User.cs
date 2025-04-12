using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YourSoulApp.Models
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // In a real app, this should be hashed
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = "Other"; // "Male", "Female", "Other"
        public string Bio { get; set; } = string.Empty;
        public string ProfileImagePath { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;

        // Dating preferences
        public string InterestedIn { get; set; } = "Both"; // "Male", "Female", "Both"

        private int _minAgePreference = 18;
        public int MinAgePreference
        {
            get => _minAgePreference;
            set
            {
                if (_minAgePreference != value)
                {
                    _minAgePreference = value;
                    OnPropertyChanged(nameof(MinAgePreference));

                    // Ensure MaxAgePreference is at least MinAgePreference
                    if (MaxAgePreference < value)
                    {
                        MaxAgePreference = value;
                    }
                }
            }
        }

        private int _maxAgePreference = 100;
        public int MaxAgePreference
        {
            get => _maxAgePreference;
            set
            {
                if (_maxAgePreference != value)
                {
                    _maxAgePreference = value;
                    OnPropertyChanged(nameof(MaxAgePreference));
                }
            }
        }

        public int MaxDistance { get; set; } = 50; // in kilometers

        // These properties are not stored in the database but used for UI
        [Ignore]
        public ObservableCollection<string> Photos { get; set; } = new ObservableCollection<string>();

        [Ignore]
        public bool IsCurrentUser { get; set; }

        private double _distance;
        [Ignore]
        public double Distance
        {
            get => _distance;
            set
            {
                if (_distance != value)
                {
                    _distance = value;
                    OnPropertyChanged(nameof(Distance));
                }
            }
        }

        // Helper method for property change notification
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
