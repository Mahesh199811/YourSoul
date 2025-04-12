using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using YourSoulApp.Models;
using YourSoulApp.Services;
using System.Windows.Input;

namespace YourSoulApp.ViewModels
{
    public partial class NearbyViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly LocationService _locationService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<User> _nearbyUsers = new();

        [ObservableProperty]
        private double _searchRadius = 50;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private Color _statusMessageColor = Colors.Black;

        [ObservableProperty]
        private FontAttributes _statusMessageFontAttributes = FontAttributes.None;

        [ObservableProperty]
        private Location? _currentLocation;



        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private bool _isUserSelected;

        public NearbyViewModel(DatabaseService databaseService, AuthService authService,
            LocationService locationService, NotificationService notificationService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _locationService = locationService;
            _notificationService = notificationService;
            Title = "Nearby";
        }

        public bool IsUserLoggedIn()
        {
            return _authService.IsLoggedIn();
        }

        [RelayCommand]
        public async Task LoadNearbyUsersAsync()
        {
            Debug.WriteLine("LoadNearbyUsersAsync called");
            if (!IsUserLoggedIn())
            {
                Debug.WriteLine("User not logged in, cannot load nearby users");
                StatusMessage = "Please log in to see nearby users";
                StatusMessageColor = Colors.Red;
                return;
            }

            Debug.WriteLine("User is logged in, proceeding with loading nearby users");

            IsBusy = true;
            StatusMessage = "Finding your location...";
            StatusMessageColor = Colors.DarkBlue;
            StatusMessageFontAttributes = FontAttributes.Bold;

            try
            {
                Debug.WriteLine("Getting current location...");
                // Get current location
                CurrentLocation = await _locationService.GetCurrentLocationAsync();

                if (CurrentLocation == null)
                {
                    StatusMessage = "Unable to determine your location. Please check your location settings.";
                    StatusMessageColor = Colors.Red;
                    return;
                }

                Debug.WriteLine($"Current location: {CurrentLocation.Latitude}, {CurrentLocation.Longitude}");

                // Set map position
                if (CurrentLocation != null)
                {
                    // Calculate zoom level based on search radius
                    Debug.WriteLine($"Using current location: {CurrentLocation.Latitude}, {CurrentLocation.Longitude}");
                }
                else
                {
                    Debug.WriteLine("Current location is null");
                }

                // Get nearby users
                StatusMessage = "Finding people nearby...";
                var users = await _locationService.GetNearbyUsersAsync(SearchRadius);
                Debug.WriteLine($"Found {users.Count} nearby users");

                // Update UI
                NearbyUsers.Clear();

                // Get current user
                var currentUser = AuthService.CurrentUser;
                if (CurrentLocation != null && currentUser != null)
                {
                    Debug.WriteLine($"Current user: {currentUser.Name} at {CurrentLocation.Latitude}, {CurrentLocation.Longitude}");
                }

                // Add nearby users with distance calculation
                foreach (var user in users)
                {
                    // Calculate distance from current user
                    if (CurrentLocation != null)
                    {
                        user.Distance = _locationService.CalculateDistance(
                            CurrentLocation.Latitude, CurrentLocation.Longitude,
                            user.Latitude, user.Longitude);
                    }

                    NearbyUsers.Add(user);

                    Debug.WriteLine($"Added user {user.Name} at {user.Latitude}, {user.Longitude}");
                }

                if (NearbyUsers.Count == 0)
                {
                    StatusMessage = "No users found nearby. Try increasing your search radius.";
                    StatusMessageColor = Colors.Orange;
                }
                else
                {
                    StatusMessage = $"Found {NearbyUsers.Count} people nearby.";
                    StatusMessageColor = Colors.Green;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                StatusMessageColor = Colors.Red;
                Debug.WriteLine($"Error in LoadNearbyUsersAsync: {ex}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("LoadNearbyUsersAsync finished (IsBusy set to false)");
            }
        }

        [RelayCommand]
        private async Task ViewProfileAsync(User? user)
        {
            if (user == null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "UserId", user.Id }
            };

            await Shell.Current.GoToAsync($"userprofile", parameters);
        }

        [RelayCommand]
        private async Task LikeUserAsync(User? user)
        {
            if (user == null || !_authService.IsLoggedIn())
                return;

            try
            {
                var currentUser = AuthService.CurrentUser;
                bool isMatch = await _databaseService.LikeUserAsync(currentUser.Id, user.Id);

                if (isMatch)
                {
                    // It's a mutual match!
                    await _notificationService.SendNewMatchNotificationAsync(user.Id);
                    await Shell.Current.DisplayAlert("It's a Match!",
                        $"You and {user.Name} liked each other!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Like Sent",
                        $"You liked {user.Name}. If they like you back, it will be a match!", "OK");
                }

                // Refresh the list
                await LoadNearbyUsersAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void SelectUser(User? user)
        {
            SelectedUser = user;
            IsUserSelected = user != null;
        }

        [RelayCommand]
        private void ClearSelection()
        {
            SelectedUser = null;
            IsUserSelected = false;
        }

        [RelayCommand]
        private async Task UpdateSearchRadiusAsync()
        {
            await LoadNearbyUsersAsync();
        }
    }
}
