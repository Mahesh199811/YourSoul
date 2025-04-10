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
    [QueryProperty(nameof(MatchedUserId), "MatchedUserId")]
    public partial class ChatDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private int _matchedUserId;

        [ObservableProperty]
        private User _matchedUser;

        [ObservableProperty]
        private ObservableCollection<Message> _messages;

        [ObservableProperty]
        private string _newMessage;

        [ObservableProperty]
        private bool _hasMessages;

        [ObservableProperty]
        private string _statusMessage;

        public ChatDetailViewModel(DatabaseService databaseService, AuthService authService, NotificationService notificationService)
        {
            _databaseService = databaseService;
            _authService = authService;
            _notificationService = notificationService;
            Messages = new ObservableCollection<Message>();
        }

        public async Task LoadChatAsync()
        {
            if (!_authService.IsLoggedIn() || MatchedUserId <= 0)
                return;

            IsBusy = true;
            StatusMessage = "Loading chat...";

            try
            {
                var currentUser = AuthService.CurrentUser;
                MatchedUser = await _databaseService.GetUserAsync(MatchedUserId);

                if (MatchedUser != null)
                {
                    Title = MatchedUser.Name;

                    var messages = await _databaseService.GetMessagesAsync(currentUser.Id, MatchedUserId);

                    // Mark messages as read
                    await _databaseService.MarkMessagesAsReadAsync(MatchedUserId, currentUser.Id);

                    // Set IsFromCurrentUser flag for UI
                    foreach (var message in messages)
                    {
                        message.IsFromCurrentUser = message.SenderId == currentUser.Id;
                    }

                    Messages.Clear();
                    foreach (var message in messages)
                    {
                        Messages.Add(message);
                    }

                    HasMessages = Messages.Any();

                    if (!HasMessages)
                    {
                        StatusMessage = $"No messages yet. Say hello to {MatchedUser.Name}!";
                    }
                    else
                    {
                        StatusMessage = string.Empty;
                    }
                }
                else
                {
                    StatusMessage = "User not found.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading chat: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(NewMessage) || !_authService.IsLoggedIn() || MatchedUser == null)
                return;

            try
            {
                var currentUser = AuthService.CurrentUser;

                var message = new Message
                {
                    SenderId = currentUser.Id,
                    ReceiverId = MatchedUser.Id,
                    Content = NewMessage,
                    SentDate = DateTime.Now,
                    IsRead = false,
                    IsFromCurrentUser = true
                };

                await _databaseService.SaveMessageAsync(message);

                // Add to local messages
                Messages.Add(message);
                HasMessages = true;
                StatusMessage = string.Empty;

                // Clear input
                NewMessage = string.Empty;

                // Send notification (in a real app, this would be handled by a server)
                await _notificationService.SendNewMessageNotificationAsync(message);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RefreshChatAsync()
        {
            await LoadChatAsync();
        }

        [RelayCommand]
        private async Task ViewProfileAsync()
        {
            if (MatchedUser == null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "UserId", MatchedUser.Id }
            };

            await Shell.Current.GoToAsync($"userprofile", parameters);
        }
    }
}
