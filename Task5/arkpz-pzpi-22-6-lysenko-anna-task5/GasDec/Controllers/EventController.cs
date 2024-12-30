using GasDec.Models;
using GasDec.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GasDec.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }
        
        /// <summary>
        /// Отримує всі події, доступні в системі.
        /// </summary>
        /// <returns>Список подій.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати всі події.")]

        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        /// <summary>
        /// Отримує подію за її унікальним ідентифікатором.
        /// </summary>
        /// <param name="id">Унікальний ідентифікатор події.</param>
        /// <returns>Подія або повідомлення про помилку, якщо подію не знайдено.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати певну подію.")]
        public async Task<ActionResult<Event>> GetEventById(int id)
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound($"Подію з ID {id} не знайдено");
            }
            return Ok(eventEntity);
        }

        /// <summary>
        /// Створює нову подію у системі.
        /// </summary>
        /// <param name="newEvent">Об'єкт події, що містить дані для створення.</param>
        /// <returns>Повертає створену подію із унікальним ідентифікатором.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Створити нову подію.")]
        
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event newEvent)
        {
            var createdEvent = await _eventService.CreateEventAsync(newEvent);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.event_id }, createdEvent);
        }

        /// <summary>
        /// Оновлює дані існуючої події за її ідентифікатором.
        /// </summary>
        /// <param name="id">Унікальний ідентифікатор події.</param>
        /// <param name="updatedEvent">Об'єкт події з оновленими даними.</param>
        /// <returns>Статус успішного оновлення або повідомлення про помилку.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Оновити обрану подію.")]
        
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
        {
            try
            {
                await _eventService.UpdateEventAsync(id, updatedEvent);
                return Ok("Подію оновлено успішно.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Видаляє подію за її унікальним ідентифікатором.
        /// </summary>
        /// <param name="id">Унікальний ідентифікатор події.</param>
        /// <returns>Статус успішного видалення або повідомлення про помилку.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Видалити обрану подію.")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return Ok("Подію видалено успішно.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Отримує всі події з вказаним рівнем важливості.
        /// </summary>
        /// <param name="severity">Рівень важливості подій (Low, Medium, High).</param>
        /// <returns>Список подій або повідомлення про відсутність таких подій.</returns>
        [HttpGet("severity/{severity}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати всі події з обраною важливістю.")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsBySeverity(string severity)
        {
            if (!Enum.TryParse(severity, true, out SeverityLevel severityLevel))
            {
                return BadRequest($"Невірне значення важливості: '{severity}'." +
                    $" Доступні значення: Low, Medium, High.");
            }

            var events = await _eventService.GetEventsBySeverityAsync(severityLevel);
            
            if (events == null || events.Count == 0)
            {
                return NotFound($"Події з важливістю '{severity}' не знайдено");
            }
            
            return Ok(events);
        }
    }
}
