using Microsoft.AspNetCore.SignalR;

namespace MaidLinker.Hubs
{
    public interface INotificationService
    {
        Task SendMessage(string userId, string message);
    }
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async Task SendMessage(string userId, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage",userId, message);
        }
    }
}