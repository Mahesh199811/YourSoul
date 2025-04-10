using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using YourSoulApp.Models;
using YourSoulApp.Services;

namespace YourSoulApp.ViewModels
{
    public partial class MatchesViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private ObservableCollection<User> _mutualMatches;

        [ObservableProperty]
        private ObservableCollection<User> _pendingMatches;

        [ObservableProperty]
        private bool _hasMutualMatches;

        [ObservableProperty]
        private bool _hasPendingMatches;

        [ObservableProperty]
        private string _statusMessage;

        public MatchesViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            Title = "Matches";
            MutualMatches = new ObservableCollection<User>();
            PendingMatches = new ObservableCollection<User>();
        }

        public async Task LoadMatchesAsync()
        {
            if (!_authService.IsLoggedIn())
                return;

            IsBusy = true;
            StatusMessage = "Loading matches...";

            try
            {
                var currentUser = AuthService.CurrentUser;
                var allMatches = await _databaseService.GetUserMatchesAsync(currentUser.Id);

                MutualMatches.Clear();
                PendingMatches.Clear();

                foreach (var match in allMatches)
                {
                    int otherUserId = match.User1Id == currentUser.Id ? match.User2Id : match.User1Id;
                    var otherUser = await _databaseService.GetUserAsync(otherUserId);

                    if (match.IsMutualMatch)
                    {
                        MutualMatches.Add(otherUser);
                    }
                    else if ((match.User1Id == currentUser.Id && !match.User2LikesUser1) ||
                             (match.User2Id == currentUser.Id && !match.User1LikesUser2))
                    {
                        // Skip matches where current user liked the other person but not mutual yet
                    }
                    else
                    {
                        // This is someone who liked the current user
                        PendingMatches.Add(otherUser);
                    }
                }

                HasMutualMatches = MutualMatches.Any();
                HasPendingMatches = PendingMatches.Any();

                if (!HasMutualMatches && !HasPendingMatches)
                {
                    StatusMessage = "No matches yet. Start swiping to find matches!";
                }
                else
                {
                    StatusMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading matches: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LikeUserAsync(User user)
        {
            if (user == null || !_authService.IsLoggedIn())
                return;

            try
            {
                var currentUser = AuthService.CurrentUser;
                await _databaseService.LikeUserAsync(currentUser.Id, user.Id);

                // Refresh matches
                await LoadMatchesAsync();

                await Shell.Current.DisplayAlert("Match Created!",
                    $"You matched with {user.Name}!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task ViewProfileAsync(User user)
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
        private async Task StartChatAsync(User user)
        {
            if (user == null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "MatchedUserId", user.Id }
            };

            await Shell.Current.GoToAsync($"chatdetail", parameters);
        }

        [RelayCommand]
        private async Task RefreshMatchesAsync()
        {
            await LoadMatchesAsync();
        }
    }
}
