using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using YourSoulApp.Models;
using YourSoulApp.Services;

namespace YourSoulApp.ViewModels
{
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class UserProfileViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private int _userId;

        [ObservableProperty]
        private User _user;

        [ObservableProperty]
        private bool _isMatch;

        [ObservableProperty]
        private string _statusMessage;

        public UserProfileViewModel(DatabaseService databaseService, AuthService authService, NotificationService notificationService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _notificationService = notificationService;
            Title = "User Profile";
        }

        public async Task LoadUserProfileAsync()
        {
            if (UserId <= 0)
                return;

            IsBusy = true;
            StatusMessage = "Loading profile...";

            try
            {
                User = await _databaseService.GetUserAsync(UserId);

                if (User != null)
                {
                    Title = User.Name;

                    if (_authService.IsLoggedIn())
                    {
                        var currentUser = AuthService.CurrentUser;
                        var match = await _databaseService.GetMatchAsync(currentUser.Id, UserId);

                        IsMatch = match != null && match.IsMutualMatch;
                    }

                    StatusMessage = string.Empty;
                }
                else
                {
                    StatusMessage = "User not found.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading profile: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LikeUserAsync()
        {
            if (User == null || !_authService.IsLoggedIn())
                return;

            try
            {
                var currentUser = AuthService.CurrentUser;
                bool isNewMatch = await _databaseService.LikeUserAsync(currentUser.Id, User.Id);

                if (isNewMatch)
                {
                    IsMatch = true;
                    await _notificationService.SendNewMatchNotificationAsync(User.Id);
                    await Shell.Current.DisplayAlert("It's a Match!",
                        $"You and {User.Name} liked each other!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Like Sent",
                        $"You liked {User.Name}. If they like you back, it will be a match!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task StartChatAsync()
        {
            if (User == null || !IsMatch)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "MatchedUserId", User.Id }
            };

            await Shell.Current.GoToAsync($"chatdetail", parameters);
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
