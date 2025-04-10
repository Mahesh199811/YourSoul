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
    public partial class ChatsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        [ObservableProperty]
        private ObservableCollection<ChatConversation> _conversations;

        [ObservableProperty]
        private bool _hasConversations;

        [ObservableProperty]
        private string _statusMessage;

        public ChatsViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            Title = "Chats";
            Conversations = new ObservableCollection<ChatConversation>();
        }

        public async Task LoadConversationsAsync()
        {
            if (!_authService.IsLoggedIn())
                return;

            IsBusy = true;
            StatusMessage = "Loading conversations...";

            try
            {
                var currentUser = AuthService.CurrentUser;
                var conversations = await _databaseService.GetUserConversationsAsync(currentUser.Id);

                Conversations.Clear();
                foreach (var conversation in conversations)
                {
                    Conversations.Add(conversation);
                }

                HasConversations = Conversations.Any();

                if (!HasConversations)
                {
                    StatusMessage = "No conversations yet. Match with someone to start chatting!";
                }
                else
                {
                    StatusMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading conversations: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task OpenConversationAsync(ChatConversation conversation)
        {
            if (conversation == null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "MatchedUserId", conversation.MatchedUser.Id }
            };

            await Shell.Current.GoToAsync($"chatdetail", parameters);
        }

        [RelayCommand]
        private async Task RefreshConversationsAsync()
        {
            await LoadConversationsAsync();
        }
    }
}
