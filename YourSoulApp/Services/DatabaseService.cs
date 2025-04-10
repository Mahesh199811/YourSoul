using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YourSoulApp.Models;

namespace YourSoulApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        
        public DatabaseService()
        {
            Init();
        }
        
        private async void Init()
        {
            if (_database != null)
                return;
                
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "yoursoul.db");
            _database = new SQLiteAsyncConnection(dbPath);
            
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Match>();
            await _database.CreateTableAsync<Message>();
            
            // Add sample data if database is empty
            await SeedDatabaseAsync();
        }
        
        private async Task SeedDatabaseAsync()
        {
            // Check if we already have users
            var userCount = await _database.Table<User>().CountAsync();
            if (userCount > 0)
                return;
                
            // Add sample users
            var users = new List<User>
            {
                new User
                {
                    Username = "john",
                    Password = "password", // In a real app, this should be hashed
                    Name = "John Smith",
                    Age = 28,
                    Gender = "Male",
                    Bio = "I love hiking and photography. Looking for someone to share adventures with.",
                    ProfileImagePath = "user_male_1.png",
                    Location = "New York",
                    InterestedIn = "Female",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 30
                },
                new User
                {
                    Username = "sarah",
                    Password = "password",
                    Name = "Sarah Johnson",
                    Age = 26,
                    Gender = "Female",
                    Bio = "Coffee enthusiast, book lover, and fitness addict. Let's chat!",
                    ProfileImagePath = "user_female_1.png",
                    Location = "Boston",
                    InterestedIn = "Male",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 25
                },
                new User
                {
                    Username = "mike",
                    Password = "password",
                    Name = "Mike Wilson",
                    Age = 30,
                    Gender = "Male",
                    Bio = "Software developer by day, musician by night. Looking for someone to share my passion for music.",
                    ProfileImagePath = "user_male_2.png",
                    Location = "San Francisco",
                    InterestedIn = "Female",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 40
                },
                new User
                {
                    Username = "emily",
                    Password = "password",
                    Name = "Emily Davis",
                    Age = 25,
                    Gender = "Female",
                    Bio = "Travel addict, foodie, and yoga instructor. Let's explore the world together!",
                    ProfileImagePath = "user_female_2.png",
                    Location = "Chicago",
                    InterestedIn = "Male",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 35
                },
                new User
                {
                    Username = "alex",
                    Password = "password",
                    Name = "Alex Brown",
                    Age = 29,
                    Gender = "Male",
                    Bio = "Sports fan, dog lover, and outdoor enthusiast. Looking for someone to share adventures with.",
                    ProfileImagePath = "user_male_3.png",
                    Location = "Los Angeles",
                    InterestedIn = "Female",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 30
                }
            };
            
            foreach (var user in users)
            {
                await _database.InsertAsync(user);
            }
            
            // Create some sample matches
            var matches = new List<Match>
            {
                new Match
                {
                    User1Id = 1, // John
                    User2Id = 2, // Sarah
                    MatchDate = DateTime.Now.AddDays(-5),
                    IsMutualMatch = true,
                    User1LikesUser2 = true,
                    User2LikesUser1 = true
                },
                new Match
                {
                    User1Id = 1, // John
                    User2Id = 4, // Emily
                    MatchDate = DateTime.Now.AddDays(-2),
                    IsMutualMatch = true,
                    User1LikesUser2 = true,
                    User2LikesUser1 = true
                },
                new Match
                {
                    User1Id = 3, // Mike
                    User2Id = 2, // Sarah
                    MatchDate = DateTime.Now.AddDays(-3),
                    IsMutualMatch = false,
                    User1LikesUser2 = true,
                    User2LikesUser1 = false
                }
            };
            
            foreach (var match in matches)
            {
                await _database.InsertAsync(match);
            }
            
            // Add some sample messages
            var messages = new List<Message>
            {
                new Message
                {
                    SenderId = 1, // John
                    ReceiverId = 2, // Sarah
                    Content = "Hi Sarah, how are you?",
                    SentDate = DateTime.Now.AddDays(-5).AddHours(1),
                    IsRead = true
                },
                new Message
                {
                    SenderId = 2, // Sarah
                    ReceiverId = 1, // John
                    Content = "Hey John! I'm good, thanks for asking. How about you?",
                    SentDate = DateTime.Now.AddDays(-5).AddHours(2),
                    IsRead = true
                },
                new Message
                {
                    SenderId = 1, // John
                    ReceiverId = 2, // Sarah
                    Content = "I'm doing great! Would you like to grab coffee sometime?",
                    SentDate = DateTime.Now.AddDays(-4),
                    IsRead = true
                },
                new Message
                {
                    SenderId = 2, // Sarah
                    ReceiverId = 1, // John
                    Content = "That sounds lovely! When were you thinking?",
                    SentDate = DateTime.Now.AddDays(-3),
                    IsRead = true
                },
                new Message
                {
                    SenderId = 1, // John
                    ReceiverId = 4, // Emily
                    Content = "Hi Emily, I noticed we both like hiking. Any favorite trails?",
                    SentDate = DateTime.Now.AddDays(-2).AddHours(3),
                    IsRead = true
                },
                new Message
                {
                    SenderId = 4, // Emily
                    ReceiverId = 1, // John
                    Content = "Hey John! Yes, I love the mountain trails. How about you?",
                    SentDate = DateTime.Now.AddDays(-2).AddHours(4),
                    IsRead = false
                }
            };
            
            foreach (var message in messages)
            {
                await _database.InsertAsync(message);
            }
        }
        
        // User methods
        public async Task<User> GetUserAsync(int id)
        {
            return await _database.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }
        
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _database.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
        }
        
        public async Task<int> SaveUserAsync(User user)
        {
            if (user.Id != 0)
                return await _database.UpdateAsync(user);
            else
                return await _database.InsertAsync(user);
        }
        
        public async Task<List<User>> GetPotentialMatchesAsync(User currentUser)
        {
            // Get users based on gender preference
            var query = _database.Table<User>().Where(u => u.Id != currentUser.Id);
            
            if (currentUser.InterestedIn != "Both")
            {
                query = query.Where(u => u.Gender == currentUser.InterestedIn);
            }
            
            // Filter by age preference
            query = query.Where(u => u.Age >= currentUser.MinAgePreference && u.Age <= currentUser.MaxAgePreference);
            
            // Get all potential matches
            var potentialMatches = await query.ToListAsync();
            
            // Get existing matches to filter out
            var existingMatches = await GetUserMatchesAsync(currentUser.Id);
            var matchedUserIds = existingMatches.Select(m => 
                m.User1Id == currentUser.Id ? m.User2Id : m.User1Id).ToList();
            
            // Filter out users that are already matched
            return potentialMatches.Where(u => !matchedUserIds.Contains(u.Id)).ToList();
        }
        
        // Match methods
        public async Task<List<Match>> GetUserMatchesAsync(int userId)
        {
            return await _database.Table<Match>()
                .Where(m => (m.User1Id == userId || m.User2Id == userId))
                .ToListAsync();
        }
        
        public async Task<List<Match>> GetUserMutualMatchesAsync(int userId)
        {
            return await _database.Table<Match>()
                .Where(m => (m.User1Id == userId || m.User2Id == userId) && m.IsMutualMatch)
                .ToListAsync();
        }
        
        public async Task<Match> GetMatchAsync(int user1Id, int user2Id)
        {
            return await _database.Table<Match>()
                .Where(m => (m.User1Id == user1Id && m.User2Id == user2Id) || 
                           (m.User1Id == user2Id && m.User2Id == user1Id))
                .FirstOrDefaultAsync();
        }
        
        public async Task<int> SaveMatchAsync(Match match)
        {
            if (match.Id != 0)
                return await _database.UpdateAsync(match);
            else
                return await _database.InsertAsync(match);
        }
        
        public async Task<bool> LikeUserAsync(int currentUserId, int likedUserId)
        {
            var existingMatch = await GetMatchAsync(currentUserId, likedUserId);
            bool isNewMutualMatch = false;
            
            if (existingMatch == null)
            {
                // Create new match
                var newMatch = new Match
                {
                    User1Id = currentUserId,
                    User2Id = likedUserId,
                    MatchDate = DateTime.Now,
                    IsMutualMatch = false,
                    User1LikesUser2 = true,
                    User2LikesUser1 = false
                };
                
                await SaveMatchAsync(newMatch);
            }
            else
            {
                // Update existing match
                if (existingMatch.User1Id == currentUserId)
                {
                    existingMatch.User1LikesUser2 = true;
                    if (existingMatch.User2LikesUser1)
                    {
                        existingMatch.IsMutualMatch = true;
                        isNewMutualMatch = true;
                    }
                }
                else
                {
                    existingMatch.User2LikesUser1 = true;
                    if (existingMatch.User1LikesUser2)
                    {
                        existingMatch.IsMutualMatch = true;
                        isNewMutualMatch = true;
                    }
                }
                
                await SaveMatchAsync(existingMatch);
            }
            
            return isNewMutualMatch;
        }
        
        // Message methods
        public async Task<List<Message>> GetMessagesAsync(int user1Id, int user2Id)
        {
            return await _database.Table<Message>()
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) || 
                           (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.SentDate)
                .ToListAsync();
        }
        
        public async Task<int> SaveMessageAsync(Message message)
        {
            if (message.Id != 0)
                return await _database.UpdateAsync(message);
            else
                return await _database.InsertAsync(message);
        }
        
        public async Task<List<ChatConversation>> GetUserConversationsAsync(int userId)
        {
            var conversations = new List<ChatConversation>();
            var matches = await GetUserMutualMatchesAsync(userId);
            
            foreach (var match in matches)
            {
                int otherUserId = match.User1Id == userId ? match.User2Id : match.User1Id;
                var otherUser = await GetUserAsync(otherUserId);
                
                var messages = await GetMessagesAsync(userId, otherUserId);
                var lastMessage = messages.OrderByDescending(m => m.SentDate).FirstOrDefault();
                
                var conversation = new ChatConversation
                {
                    MatchId = match.Id,
                    MatchedUser = otherUser,
                    Messages = new System.Collections.ObjectModel.ObservableCollection<Message>(messages),
                    LastMessageDate = lastMessage?.SentDate ?? match.MatchDate,
                    LastMessageText = lastMessage?.Content ?? "You matched with " + otherUser.Name,
                    UnreadCount = messages.Count(m => m.ReceiverId == userId && !m.IsRead)
                };
                
                conversations.Add(conversation);
            }
            
            return conversations.OrderByDescending(c => c.LastMessageDate).ToList();
        }
        
        public async Task MarkMessagesAsReadAsync(int senderId, int receiverId)
        {
            var unreadMessages = await _database.Table<Message>()
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();
                
            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                await SaveMessageAsync(message);
            }
        }
    }
}
