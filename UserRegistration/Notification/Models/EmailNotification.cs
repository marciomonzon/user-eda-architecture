using Notification.Enum;

namespace Notification.Models
{
    public class EmailNotification
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
