using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading.Tasks;
using YourSoulApp.Models;
using YourSoulApp.Services;

namespace YourSoulApp.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        private User? _currentUser;

        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        private bool _isEditing;

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (SetProperty(ref _isEditing, value))
                {
                    OnPropertyChanged(nameof(EditButtonText));
                }
            }
        }

        public string EditButtonText => IsEditing ? "Save" : "Edit";

        [RelayCommand]
        private async Task EditSaveAsync()
        {
            if (IsEditing)
            {
                // Save the profile
                await SaveProfileAsync();
            }
            else
            {
                // Toggle edit mode
                ToggleEditMode();
            }
        }

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

                    // Validate profile image path
                    if (!string.IsNullOrEmpty(CurrentUser.ProfileImagePath) &&
                        !CurrentUser.ProfileImagePath.StartsWith("user_") &&
                        !File.Exists(CurrentUser.ProfileImagePath))
                    {
                        // Reset to default image if file doesn't exist
                        CurrentUser.ProfileImagePath = CurrentUser.Gender == "Male" ? "user_male_1.png" : "user_female_1.png";
                        await _databaseService.SaveUserAsync(CurrentUser);
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

                // Reload the user data from the database to ensure UI is updated
                await _authService.UpdateCurrentUserAsync();
                CurrentUser = AuthService.CurrentUser;
                if (CurrentUser != null)
                {
                    CurrentUser.IsCurrentUser = true;
                }
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
            // Force a complete reload of the profile data
            await _authService.UpdateCurrentUserAsync();
            await LoadProfileAsync();
        }

        [RelayCommand]
        private async Task ChangeProfilePictureAsync()
        {
            if (CurrentUser == null || !IsEditing)
                return;

            try
            {
                // Check if we have the necessary permissions on Android
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.StorageRead>();
                        if (status != PermissionStatus.Granted)
                        {
                            StatusMessage = "Storage permission is required to change profile picture.";
                            return;
                        }
                    }

                    var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                    if (cameraStatus != PermissionStatus.Granted)
                    {
                        cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                    }
                }

                // Use MediaPicker to select an image
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select Profile Picture"
                });

                if (result != null)
                {
                    try
                    {
                        // Get the file path
                        var stream = await result.OpenReadAsync();

                        // Create a unique filename in the app's cache directory (more reliable on Android)
                        string fileName = $"profile_{CurrentUser.Id}_{DateTime.Now.Ticks}.jpg";
                        string cacheDir = FileSystem.CacheDirectory;
                        string filePath = Path.Combine(cacheDir, fileName);

                        // Ensure directory exists
                        Directory.CreateDirectory(cacheDir);

                        // Save the file to the cache directory
                        using (var fileStream = File.OpenWrite(filePath))
                        {
                            await stream.CopyToAsync(fileStream);
                        }

                        // Delete old profile image if it's a custom one (not a default image)
                        if (!string.IsNullOrEmpty(CurrentUser.ProfileImagePath) &&
                            !CurrentUser.ProfileImagePath.StartsWith("user_") &&
                            File.Exists(CurrentUser.ProfileImagePath))
                        {
                            try
                            {
                                File.Delete(CurrentUser.ProfileImagePath);
                            }
                            catch
                            {
                                // Ignore errors when deleting old file
                            }
                        }

                        // Update the user's profile image path
                        CurrentUser.ProfileImagePath = filePath;

                        // Save the changes to the database immediately
                        await _databaseService.SaveUserAsync(CurrentUser);

                        // Reload the user data to ensure UI is updated
                        await _authService.UpdateCurrentUserAsync();
                        CurrentUser = AuthService.CurrentUser;
                        if (CurrentUser != null)
                        {
                            CurrentUser.IsCurrentUser = true;
                        }

                        // Notify that the property has changed
                        OnPropertyChanged(nameof(CurrentUser));

                        StatusMessage = "Profile picture updated successfully.";
                    }
                    catch (Exception ex)
                    {
                        StatusMessage = $"Error processing image: {ex.Message}";
                        System.Diagnostics.Debug.WriteLine($"Image processing error: {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error selecting profile picture: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Profile picture selection error: {ex}");
            }
        }
    }
}
