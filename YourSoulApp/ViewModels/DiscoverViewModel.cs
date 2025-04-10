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
    public partial class DiscoverViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly NotificationService _notificationService;
        
        [ObservableProperty]
        private ObservableCollection<User> _potentialMatches;
        
        [ObservableProperty]
        private User _currentPotentialMatch;
        
        [ObservableProperty]
        private bool _noMoreMatches;
        
        [ObservableProperty]
        private string _statusMessage;
        
        public DiscoverViewModel(DatabaseService databaseService, AuthService authService, NotificationService notificationService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _notificationService = notificationService;
            Title = "Discover";
            PotentialMatches = new ObservableCollection<User>();
        }
        
        public async Task LoadPotentialMatchesAsync()
        {
            if (!_authService.IsLoggedIn())
                return;
                
            IsBusy = true;
            StatusMessage = "Loading potential matches...";
            
            try
            {
                var currentUser = AuthService.CurrentUser;
                var matches = await _databaseService.GetPotentialMatchesAsync(currentUser);
                
                PotentialMatches.Clear();
                foreach (var match in matches)
                {
                    PotentialMatches.Add(match);
                }
                
                NoMoreMatches = !PotentialMatches.Any();
                CurrentPotentialMatch = PotentialMatches.FirstOrDefault();
                
                if (NoMoreMatches)
                {
                    StatusMessage = "No more potential matches found. Check back later!";
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
        private async Task LikeCurrentUserAsync()
        {
            if (CurrentPotentialMatch == null || !_authService.IsLoggedIn())
                return;
                
            try
            {
                var currentUser = AuthService.CurrentUser;
                bool isMatch = await _databaseService.LikeUserAsync(currentUser.Id, CurrentPotentialMatch.Id);
                
                if (isMatch)
                {
                    // It's a mutual match!
                    await _notificationService.SendNewMatchNotificationAsync(CurrentPotentialMatch.Id);
                    await Shell.Current.DisplayAlert("It's a Match!", 
                        $"You and {CurrentPotentialMatch.Name} liked each other!", "OK");
                }
                
                // Move to next potential match
                PotentialMatches.Remove(CurrentPotentialMatch);
                CurrentPotentialMatch = PotentialMatches.FirstOrDefault();
                
                NoMoreMatches = !PotentialMatches.Any();
                if (NoMoreMatches)
                {
                    StatusMessage = "No more potential matches found. Check back later!";
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
        
        [RelayCommand]
        private void DislikeCurrentUser()
        {
            if (CurrentPotentialMatch == null)
                return;
                
            // Simply remove from potential matches and move to next
            PotentialMatches.Remove(CurrentPotentialMatch);
            CurrentPotentialMatch = PotentialMatches.FirstOrDefault();
            
            NoMoreMatches = !PotentialMatches.Any();
            if (NoMoreMatches)
            {
                StatusMessage = "No more potential matches found. Check back later!";
            }
        }
        
        [RelayCommand]
        private async Task RefreshMatchesAsync()
        {
            await LoadPotentialMatchesAsync();
        }
    }
}
