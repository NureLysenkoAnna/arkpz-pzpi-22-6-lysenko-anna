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

        public async Task<List<Location>> GetLocationsByFloorAsync(int floor)
        {
            return await _context.Locations
                                 .Where(l => l.floor == floor)
                                 .ToListAsync();
        }

        public async Task<List<Location>> GetLocationsByTypeAsync(string type)
        {
            return await _context.Locations
                .Where(l => l.location_type.ToLower() == type.ToLower()).ToListAsync();
        }
    }
}

