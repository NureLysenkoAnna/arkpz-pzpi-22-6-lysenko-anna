using GasDec.Models;
using GasDec.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GasDec.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Отримати всі сповіщення.")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Отримати певне сповіщення.")]
        public async Task<ActionResult<Notification>> GetNotificationById(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound($"Сповіщення з ID {id} не знайдено");
            }
            return Ok(notification);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Створити нове сповіщення.")]
        public async Task<ActionResult<Notification>> CreateNotification([FromBody] Notification newNotification)
        {
            var createdNotification = await _notificationService.CreateNotificationAsync(newNotification);
            return CreatedAtAction(nameof(GetNotificationById), 
                new { id = createdNotification.notification_id }, createdNotification);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Оновити обране сповіщення.")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] Notification updatedNotification)
        {
            try
            {
                await _notificationService.UpdateNotificationAsync(id, updatedNotification);
                return Ok("Сповіщення оновлено успішно.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Видалити обране сповіщення.")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(id);
                return Ok("Сповіщення видалено.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("type/{type}")]
        [SwaggerOperation(Summary = "Отримати всі сповіщення за вказаним типом.")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByType(string type)
        {
            var notifications = await _notificationService.GetNotificationsByTypeAsync(type);
            if (notifications == null || notifications.Count == 0)
            {
                return NotFound($"Сповіщення типу '{type}' не знайдено");
            }
            return Ok(notifications);
        }
    }
}
