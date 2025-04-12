using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YourSoulApp.Helpers;
using YourSoulApp.Models;

namespace YourSoulApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        private bool _isInitialized = false;
        private readonly TaskCompletionSource<bool> _initializationCompletionSource;

        public DatabaseService()
        {
            _initializationCompletionSource = new TaskCompletionSource<bool>();
            InitAsync();
        }

        public Task WaitForInitializationAsync()
        {
            if (_isInitialized)
                return Task.CompletedTask;

            return _initializationCompletionSource.Task;
        }

        private async void InitAsync()
        {
            try
            {
                if (_database != null)
                {
                    _isInitialized = true;
                    _initializationCompletionSource.SetResult(true);
                    return;
                }

                try
                {
                    // Ensure the directory exists
                    string dbFolder = FileSystem.AppDataDirectory;
                    Directory.CreateDirectory(dbFolder);

                    string dbPath = Path.Combine(dbFolder, "yoursoul.db");
                    System.Diagnostics.Debug.WriteLine($"Database path: {dbPath}");

                    // Create a SQLite connection directly with the path
                    _database = new SQLiteAsyncConnection(dbPath);

                    await _database.CreateTableAsync<User>();
                    await _database.CreateTableAsync<Match>();
                    await _database.CreateTableAsync<Message>();

                    // Add sample data if database is empty
                    await SeedDatabaseAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex}");
                    throw; // Re-throw to be caught by the outer try-catch
                }

                _isInitialized = true;
                _initializationCompletionSource.SetResult(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex}");
                _initializationCompletionSource.SetException(ex);
            }
        }

        private async Task SeedDatabaseAsync()
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            // Check if we already have users
            var userCount = await _database.Table<User>().CountAsync();
            if (userCount > 0)
                return;

            // Add sample users with hashed passwords
            var users = new List<User>
            {
                new User
                {
                    Username = "john",
                    Password = PasswordHasher.HashPassword("password"),
                    Name = "John Smith",
                    Age = 28,
                    Gender = "Male",
                    Bio = "I love hiking and photography. Looking for someone to share adventures with.",
                    ProfileImagePath = "user_male_1.png",
                    Location = "New York",
                    Latitude = 40.7128,
                    Longitude = -74.0060,
                    InterestedIn = "Female",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 30
                },
                new User
                {
                    Username = "sarah",
                    Password = PasswordHasher.HashPassword("password"),
                    Name = "Sarah Johnson",
                    Age = 26,
                    Gender = "Female",
                    Bio = "Coffee enthusiast, book lover, and fitness addict. Let's chat!",
                    ProfileImagePath = "user_female_1.png",
                    Location = "Boston",
                    Latitude = 42.3601,
                    Longitude = -71.0589,
                    InterestedIn = "Male",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 25
                },
                new User
                {
                    Username = "mike",
                    Password = PasswordHasher.HashPassword("password"),
                    Name = "Mike Wilson",
                    Age = 30,
                    Gender = "Male",
                    Bio = "Software developer by day, musician by night. Looking for someone to share my passion for music.",
                    ProfileImagePath = "user_male_2.png",
                    Location = "San Francisco",
                    Latitude = 37.7749,
                    Longitude = -122.4194,
                    InterestedIn = "Female",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 40
                },
                new User
                {
                    Username = "emily",
                    Password = PasswordHasher.HashPassword("password"),
                    Name = "Emily Davis",
                    Age = 25,
                    Gender = "Female",
                    Bio = "Travel addict, foodie, and yoga instructor. Let's explore the world together!",
                    ProfileImagePath = "user_female_2.png",
                    Location = "Chicago",
                    Latitude = 41.8781,
                    Longitude = -87.6298,
                    InterestedIn = "Male",
                    MinAgePreference = 25,
                    MaxAgePreference = 35,
                    MaxDistance = 35
                },
                new User
                {
                    Username = "alex",
                    Password = PasswordHasher.HashPassword("password"),
                    Name = "Alex Brown",
                    Age = 29,
                    Gender = "Male",
                    Bio = "Sports fan, dog lover, and outdoor enthusiast. Looking for someone to share adventures with.",
                    ProfileImagePath = "user_male_3.png",
                    Location = "Los Angeles",
                    Latitude = 34.0522,
                    Longitude = -118.2437,
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
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            return await _database.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            return await _database.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserAsync(User user)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            if (user.Id != 0)
                return await _database.UpdateAsync(user);
            else
                return await _database.InsertAsync(user);
        }

        public async Task<List<User>> GetPotentialMatchesAsync(User currentUser)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            // Get existing matches to filter out (do this first to avoid processing users we've already matched with)
            var existingMatches = await GetUserMatchesAsync(currentUser.Id);
            var matchedUserIds = existingMatches.Select(m =>
                m.User1Id == currentUser.Id ? m.User2Id : m.User1Id).ToList();

            // Build a more efficient query that includes all filtering conditions
            var query = _database.Table<User>()
                .Where(u => u.Id != currentUser.Id)  // Not the current user
                .Where(u => !matchedUserIds.Contains(u.Id));  // Not already matched

            // Filter by current user's gender preference
            if (currentUser.InterestedIn != "Both")
            {
                query = query.Where(u => u.Gender == currentUser.InterestedIn);
            }

            // Filter by current user's age preference
            query = query.Where(u => u.Age >= currentUser.MinAgePreference &&
                                    u.Age <= currentUser.MaxAgePreference);

            // Get potential matches after initial filtering
            var potentialMatches = await query.ToListAsync();

            // Create a more efficient list for the final result
            var mutualPreferenceMatches = new List<User>(potentialMatches.Count);

            // Optimize the loop for better performance
            foreach (var match in potentialMatches)
            {
                // Check mutual preference conditions
                if ((match.InterestedIn == "Both" || match.InterestedIn == currentUser.Gender) &&
                    (currentUser.Age >= match.MinAgePreference && currentUser.Age <= match.MaxAgePreference))
                {
                    mutualPreferenceMatches.Add(match);
                }
            }

            return mutualPreferenceMatches;
        }

        // Match methods
        public async Task<List<Match>> GetUserMatchesAsync(int userId)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            return await _database.Table<Match>()
                .Where(m => m.User1Id == userId || m.User2Id == userId)
                .ToListAsync();
        }

        public async Task<List<Match>> GetUserMutualMatchesAsync(int userId)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            return await _database.Table<Match>()
                .Where(m => (m.User1Id == userId || m.User2Id == userId) && m.IsMutualMatch)
                .ToListAsync();
        }

        public async Task<Match> GetMatchAsync(int user1Id, int user2Id)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            return await _database.Table<Match>()
                .Where(m => (m.User1Id == user1Id && m.User2Id == user2Id) ||
                           (m.User1Id == user2Id && m.User2Id == user1Id))
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveMatchAsync(Match match)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            if (match.Id != 0)
                return await _database.UpdateAsync(match);
            else
                return await _database.InsertAsync(match);
        }

        public async Task<bool> LikeUserAsync(int currentUserId, int likedUserId)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

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
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            return await _database.Table<Message>()
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                           (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.SentDate)
                .ToListAsync();
        }

        public async Task<int> SaveMessageAsync(Message message)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            if (message.Id != 0)
                return await _database.UpdateAsync(message);
            else
                return await _database.InsertAsync(message);
        }

        public async Task<List<ChatConversation>> GetUserConversationsAsync(int userId)
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

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
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            var unreadMessages = await _database.Table<Message>()
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                await SaveMessageAsync(message);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            if (_database == null)
                throw new InvalidOperationException("Database is not initialized");

            return await _database.Table<User>().ToListAsync();
        }
    }
}
