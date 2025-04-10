using System;
using System.Threading.Tasks;
using YourSoulApp.Models;

namespace YourSoulApp.Services
{
    public class AuthService
    {
        private readonly DatabaseService _databaseService;
        private static User _currentUser;
        
        public static User CurrentUser => _currentUser;
        
        public event EventHandler<User> UserLoggedIn;
        public event EventHandler UserLoggedOut;
        
        public AuthService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        
        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _databaseService.GetUserByUsernameAsync(username);
            
            if (user != null && user.Password == password) // In a real app, use proper password hashing
            {
                _currentUser = user;
                UserLoggedIn?.Invoke(this, user);
                return true;
            }
            
            return false;
        }
        
        public async Task<bool> RegisterAsync(User user)
        {
            // Check if username already exists
            var existingUser = await _databaseService.GetUserByUsernameAsync(user.Username);
            
            if (existingUser != null)
                return false;
                
            await _databaseService.SaveUserAsync(user);
            
            // Auto login after registration
            _currentUser = user;
            UserLoggedIn?.Invoke(this, user);
            
            return true;
        }
        
        public void Logout()
        {
            _currentUser = null;
            UserLoggedOut?.Invoke(this, EventArgs.Empty);
        }
        
        public bool IsLoggedIn()
        {
            return _currentUser != null;
        }
        
        public async Task UpdateCurrentUserAsync()
        {
            if (_currentUser != null)
            {
                _currentUser = await _databaseService.GetUserAsync(_currentUser.Id);
            }
        }
    }
}
