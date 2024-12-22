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

        [HttpGet]
        [SwaggerOperation(Summary = "Отримати всі події.")]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
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

        [HttpPost]
        [SwaggerOperation(Summary = "Створити нову подію.")]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event newEvent)
        {
            var createdEvent = await _eventService.CreateEventAsync(newEvent);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.event_id }, createdEvent);
        }

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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

        [HttpGet("severity/{severity}")]
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
