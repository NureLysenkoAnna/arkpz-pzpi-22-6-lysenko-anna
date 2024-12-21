using GasDec.Models;
using Microsoft.EntityFrameworkCore;

namespace GasDec.Services
{
    public class EventService
    {
        private readonly GasLeakDbContext _context;

        public EventService(GasLeakDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                                 .Include(e => e.SensorData)
                                 .ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int eventId)
        {
            return await _context.Events
                                 .Include(e => e.SensorData)
                                 .FirstOrDefaultAsync(e => e.event_id == eventId);
        }

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }

        public async Task<Event> UpdateEventAsync(int eventId, Event updatedEvent)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
            {
                throw new Exception("Подію не знайдено");
            }

            existingEvent.event_time = updatedEvent.event_time;
            existingEvent.severity = updatedEvent.severity;
            existingEvent.data_id = updatedEvent.data_id;

            await _context.SaveChangesAsync();
            return existingEvent;
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var eventToDelete = await _context.Events.FindAsync(eventId);
            if (eventToDelete == null)
            {
                throw new Exception("Подію не знайдено");
            }

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Event>> GetEventsBySeverityAsync(SeverityLevel severity)
        {
            return await _context.Events
                        .Where(e => e.severity == severity)
                        .Include(e => e.SensorData)
                        .ToListAsync();
        }
    }
}
