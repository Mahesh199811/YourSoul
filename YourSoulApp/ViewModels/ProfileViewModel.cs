using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using YourSoulApp.Models;
using YourSoulApp.Services;

namespace YourSoulApp.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        
        [ObservableProperty]
        private User _currentUser;
        
        [ObservableProperty]
        private string _statusMessage;
        
        [ObservableProperty]
        private bool _isEditing;
        
        public ProfileViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            Title = "Profile";
        }
        
        public async Task LoadProfileAsync()
        {
            if (!_authService.IsLoggedIn())
                return;
                
            IsBusy = true;
            StatusMessage = "Loading profile...";
            
            try
            {
                await _authService.UpdateCurrentUserAsync();
                CurrentUser = AuthService.CurrentUser;
                
                if (CurrentUser != null)
                {
                    CurrentUser.IsCurrentUser = true;
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
        private void ToggleEditMode()
        {
            IsEditing = !IsEditing;
        }
        
        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (CurrentUser == null)
                return;
                
            IsBusy = true;
            StatusMessage = "Saving profile...";
            
            try
            {
                await _databaseService.SaveUserAsync(CurrentUser);
                IsEditing = false;
                StatusMessage = "Profile saved successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving profile: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
            
            if (confirm)
            {
                _authService.Logout();
                // Navigation will be handled by App.xaml.cs when UserLoggedOut event is fired
            }
        }
        
        [RelayCommand]
        private async Task RefreshProfileAsync()
        {
            await LoadProfileAsync();
        }
    }
}
