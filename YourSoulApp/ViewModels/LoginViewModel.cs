using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using YourSoulApp.Services;

namespace YourSoulApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        [ObservableProperty]
        private string _username;
        
        [ObservableProperty]
        private string _password;
        
        [ObservableProperty]
        private string _errorMessage;
        
        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Login";
        }
        
        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password are required.";
                return;
            }
            
            IsBusy = true;
            ErrorMessage = string.Empty;
            
            try
            {
                bool success = await _authService.LoginAsync(Username, Password);
                
                if (!success)
                {
                    ErrorMessage = "Invalid username or password.";
                }
                else
                {
                    // Navigation will be handled by App.xaml.cs when UserLoggedIn event is fired
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//register");
        }
    }
}
