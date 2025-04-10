using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using YourSoulApp.Models;
using YourSoulApp.Services;

namespace YourSoulApp.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        [ObservableProperty]
        private string _username;
        
        [ObservableProperty]
        private string _password;
        
        [ObservableProperty]
        private string _confirmPassword;
        
        [ObservableProperty]
        private string _name;
        
        [ObservableProperty]
        private int _age;
        
        [ObservableProperty]
        private string _gender;
        
        [ObservableProperty]
        private string _interestedIn;
        
        [ObservableProperty]
        private string _bio;
        
        [ObservableProperty]
        private string _errorMessage;
        
        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Register";
            Age = 25;
            Gender = "Male";
            InterestedIn = "Female";
        }
        
        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || 
                string.IsNullOrWhiteSpace(Name) || Age < 18 || string.IsNullOrWhiteSpace(Gender) || 
                string.IsNullOrWhiteSpace(InterestedIn))
            {
                ErrorMessage = "All fields are required. Age must be at least 18.";
                return;
            }
            
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }
            
            IsBusy = true;
            ErrorMessage = string.Empty;
            
            try
            {
                var user = new User
                {
                    Username = Username,
                    Password = Password,
                    Name = Name,
                    Age = Age,
                    Gender = Gender,
                    InterestedIn = InterestedIn,
                    Bio = Bio ?? "No bio yet.",
                    ProfileImagePath = Gender == "Male" ? "user_male_1.png" : "user_female_1.png",
                    Location = "Unknown",
                    MinAgePreference = 18,
                    MaxAgePreference = 50,
                    MaxDistance = 50
                };
                
                bool success = await _authService.RegisterAsync(user);
                
                if (!success)
                {
                    ErrorMessage = "Username already exists.";
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
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
