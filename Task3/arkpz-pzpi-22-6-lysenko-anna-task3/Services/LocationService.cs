using GasDec.Models;
using Microsoft.EntityFrameworkCore;

namespace GasDec.Services
{
    public class LocationService
    {
        private readonly GasLeakDbContext _context;

        public LocationService(GasLeakDbContext context)
        {
            _context = context;
        }

        public async Task<List<Location>> GetAllLocationsAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> GetLocationByIdAsync(int locationId)
        {
            return await _context.Locations.FindAsync(locationId);
        }

        public async Task<Location> AddLocationAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location> UpdateLocationAsync(int locationId, Location updatedLocation)
        {
            var existingLocation = await _context.Locations.FindAsync(locationId);
            if (existingLocation == null)
            {
                throw new System.Exception("Локація не знайдена");
            }

            existingLocation.name = updatedLocation.name;
            existingLocation.location_type = updatedLocation.location_type;
            existingLocation.floor = updatedLocation.floor;
            existingLocation.area = updatedLocation.area;

            await _context.SaveChangesAsync();
            return existingLocation;
        }

        public async Task DeleteLocationAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);
            if (location == null)
            {
                throw new System.Exception("Локація не знайдена");
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Отримує всі локації за вказаним поверхом.
        /// </summary>
        /// <param name="floor">Номер поверху для фільтрації локацій.</param>
        /// <returns>Список локацій на вказаному поверсі.</returns>
        public async Task<List<Location>> GetLocationsByFloorAsync(int floor)
        {
            return await _context.Locations
                                 .Where(l => l.floor == floor)
                                 .ToListAsync();
        }

        /// <summary>
        /// Отримує всі локації за вказаним типом.
        /// </summary>
        /// <param name="type">Тип локації для фільтрації (регістр не має значення).</param>
        /// <returns>Список локацій з вказаним типом.</returns>
        public async Task<List<Location>> GetLocationsByTypeAsync(string type)
        {
            return await _context.Locations
                .Where(l => l.location_type.ToLower() == type.ToLower()).ToListAsync();
        }

        /// <summary>
        /// Обчислює необхідну кількість сенсорів для вказаної локації.
        /// </summary>
        /// <param name="locationId">ID локації, для якої обчислюється кількість сенсорів.</param>
        /// <param name="minRequiredSensors">Мінімально необхідна кількість сенсорів.</param>
        /// <returns>Необхідна кількість сенсорів для локації.</returns>
        /// <exception cref="ArgumentException">Кидає виключення, якщо локація не знайдена.</exception>
        public async Task<int> CalculateRequiredSensorsAsync(int locationId, int? minRequiredSensors = null)
        {
            var location = await _context.Locations.FindAsync(locationId);

            if (location == null)
            {
                throw new ArgumentException("Локація не знайдена.");
            }

            double baseDensity = _sensorDensity.ContainsKey(location.location_type)
                ? _sensorDensity[location.location_type] : 50;

            double area = location.area ?? 0;

            int baseSensorCount = (int)Math.Ceiling(area / baseDensity);

            int requiredSensorCount = minRequiredSensors.HasValue 
                ? Math.Max(baseSensorCount, minRequiredSensors.Value) 
                : baseSensorCount;

            return requiredSensorCount;
        }
    }
}

