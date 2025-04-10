using SQLite;
using System;

namespace YourSoulApp.Models
{
    public class Message
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Indexed]
        public int SenderId { get; set; }
        
        [Indexed]
        public int ReceiverId { get; set; }
        
        public string Content { get; set; }
        
        public DateTime SentDate { get; set; }
        
        public bool IsRead { get; set; }
        
        // For UI purposes
        [Ignore]
        public bool IsFromCurrentUser { get; set; }
    }
}
