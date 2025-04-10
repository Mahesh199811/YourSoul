using System;
using System.Collections.ObjectModel;

namespace YourSoulApp.Models
{
    public class ChatConversation
    {
        public int MatchId { get; set; }
        public User MatchedUser { get; set; }
        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        public DateTime LastMessageDate { get; set; }
        public string LastMessageText { get; set; }
        public int UnreadCount { get; set; }
    }
}
