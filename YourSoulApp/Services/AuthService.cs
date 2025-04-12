using System;
using System.Threading.Tasks;
using YourSoulApp.Helpers;
using YourSoulApp.Models;

namespace YourSoulApp.Services
{
    public class AuthService
    {
        private readonly DatabaseService _databaseService;
        private static User? _currentUser;

        public static User? CurrentUser => _currentUser;

        public event EventHandler<User>? UserLoggedIn;
        public event EventHandler? UserLoggedOut;

        public AuthService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            // Wait for database initialization to complete
            await _databaseService.WaitForInitializationAsync();

            var user = await _databaseService.GetUserByUsernameAsync(username);

            if (user != null)
            {
                // Check if the password is in the old plain text format
                if (user.Password.StartsWith("password") || !user.Password.Contains(":"))
                {
                    // Legacy password handling - plain text comparison
                    if (user.Password == password)
                    {
                        // Upgrade to hashed password on successful login
                        user.Password = PasswordHasher.HashPassword(password);
                        await _databaseService.SaveUserAsync(user);

                        // Make sure we have the latest user data
                        _currentUser = await _databaseService.GetUserAsync(user.Id);
                        System.Diagnostics.Debug.WriteLine($"Login successful (legacy): {_currentUser?.Name}, Age: {_currentUser?.Age}");

                        if (_currentUser != null)
                        {
                            UserLoggedIn?.Invoke(this, _currentUser);
                        }
                        else
                        {
                            // Fallback to the original user object if database fetch failed
                            _currentUser = user;
                            UserLoggedIn?.Invoke(this, user);
                        }
                        return true;
                    }
                }
                else
                {
                    // Modern password handling - verify hash
                    if (PasswordHasher.VerifyPassword(password, user.Password))
                    {
                        // Make sure we have the latest user data
                        _currentUser = await _databaseService.GetUserAsync(user.Id);
                        System.Diagnostics.Debug.WriteLine($"Login successful: {_currentUser?.Name}, Age: {_currentUser?.Age}");

                        if (_currentUser != null)
                        {
                            UserLoggedIn?.Invoke(this, _currentUser);
                        }
                        else
                        {
                            // Fallback to the original user object if database fetch failed
                            _currentUser = user;
                            UserLoggedIn?.Invoke(this, user);
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            // Wait for database initialization to complete
            await _databaseService.WaitForInitializationAsync();

            // Check if username already exists
            var existingUser = await _databaseService.GetUserByUsernameAsync(user.Username);

            if (existingUser != null)
                return false;

            // Hash the password before saving
            string plainPassword = user.Password;
            user.Password = PasswordHasher.HashPassword(plainPassword);

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
            // Wait for database initialization to complete
            await _databaseService.WaitForInitializationAsync();

            if (_currentUser != null)
            {
                // Get a fresh copy of the user data from the database
                var freshUserData = await _databaseService.GetUserAsync(_currentUser.Id);

                if (freshUserData != null)
                {
                    // Update the current user with the fresh data
                    _currentUser = freshUserData;
                    System.Diagnostics.Debug.WriteLine($"Updated current user data: {_currentUser.Name}, Age: {_currentUser.Age}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Warning: Could not find user data in database");
                }
            }
        }
    }
}
