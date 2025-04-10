using Plugin.LocalNotification;
using System;
using System.Threading.Tasks;
using YourSoulApp.Models;

namespace YourSoulApp.Services
{
    public class NotificationService
    {
        private readonly DatabaseService _databaseService;
        
        public NotificationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        
        public async Task SendNewMatchNotificationAsync(int matchedUserId)
        {
            var matchedUser = await _databaseService.GetUserAsync(matchedUserId);
            
            if (matchedUser == null)
                return;
                
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "New Match!",
                Description = $"You matched with {matchedUser.Name}! Say hello!",
                ReturningData = $"match:{matchedUserId}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };
            
            await LocalNotificationCenter.Current.Show(notification);
        }
        
        public async Task SendNewMessageNotificationAsync(Message message)
        {
            var sender = await _databaseService.GetUserAsync(message.SenderId);
            
            if (sender == null)
                return;
                
            var notification = new NotificationRequest
            {
                NotificationId = 200 + message.Id,
                Title = $"New message from {sender.Name}",
                Description = message.Content,
                ReturningData = $"message:{message.SenderId}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };
            
            await LocalNotificationCenter.Current.Show(notification);
        }
    }
}
