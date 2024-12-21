using GasDec.Models;
using Microsoft.EntityFrameworkCore;

namespace GasDec.Services
{
    public class NotificationService
    {
        private readonly GasLeakDbContext _context;

        public NotificationService(GasLeakDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications
                                 .Include(n => n.User)
                                 .Include(n => n.Event)
                                 .ToListAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications
                                 .Include(n => n.User)
                                 .Include(n => n.Event)
                                 .FirstOrDefaultAsync(n => n.notification_id == id);
        }

        public async Task<Notification> CreateNotificationAsync(Notification newNotification)
        {
            _context.Notifications.Add(newNotification);
            await _context.SaveChangesAsync();
            return newNotification;
        }

        public async Task<Notification> UpdateNotificationAsync(int id, Notification updatedNotification)
        {
            var existingNotification = await _context.Notifications.FindAsync(id);
            if (existingNotification == null)
            {
                throw new Exception("Сповіщення не знайдено");
            }

            existingNotification.message = updatedNotification.message;
            existingNotification.formation_time = updatedNotification.formation_time;
            existingNotification.notification_type = updatedNotification.notification_type;
            existingNotification.user_id = updatedNotification.user_id;
            existingNotification.event_id = updatedNotification.event_id;

            await _context.SaveChangesAsync();
            return existingNotification;
        }

        public async Task DeleteNotificationAsync(int id)
        {
            var notificationToDelete = await _context.Notifications.FindAsync(id);
            if (notificationToDelete == null)
            {
                throw new Exception("Сповіщення не знайдено");
            }

            _context.Notifications.Remove(notificationToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotificationsByTypeAsync(string notificationType)
        {
            return await _context.Notifications
                                 .Where(n => n.notification_type == notificationType)
                                 .Include(n => n.User)
                                 .Include(n => n.Event)
                                 .ToListAsync();
        }
    }
}
