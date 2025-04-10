using SQLite;
using System;

namespace YourSoulApp.Models
{
    public class Match
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Indexed]
        public int User1Id { get; set; }
        
        [Indexed]
        public int User2Id { get; set; }
        
        public DateTime MatchDate { get; set; }
        
        // Indicates if this is a mutual match (both users liked each other)
        public bool IsMutualMatch { get; set; }
        
        // For one-way likes (when one user likes another but not mutual yet)
        public bool User1LikesUser2 { get; set; }
        public bool User2LikesUser1 { get; set; }
    }
}
