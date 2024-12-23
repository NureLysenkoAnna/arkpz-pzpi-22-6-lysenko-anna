using GasDec.Models;
using Microsoft.EntityFrameworkCore;

namespace GasDec.Services
{
    public class SensorService
    {
        private readonly GasLeakDbContext _context;

        public SensorService(GasLeakDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sensor>> GetAllSensorsAsync()
        {
            return await _context.Sensors
                                 .Include(s => s.Location)
                                 .ToListAsync();
        }

        public async Task<Sensor> GetSensorByIdAsync(int sensorId)
        {
            return await _context.Sensors
                                 .Include(s => s.Location)
                                 .FirstOrDefaultAsync(s => s.sensor_id == sensorId);
        }

        public async Task<Sensor> CreateSensorAsync(Sensor sensor)
        {
            _context.Sensors.Add(sensor);
            await _context.SaveChangesAsync();
            return sensor;
        }

        public async Task<Sensor> UpdateSensorAsync(int sensorId, Sensor updatedSensor)
        {
            var existingSensor = await _context.Sensors.FindAsync(sensorId);
            if (existingSensor == null)
            {
                throw new System.Exception("Сенсор не знайдено");
            }

            existingSensor.type = updatedSensor.type;
            existingSensor.status = updatedSensor.status;
            existingSensor.installation_date = updatedSensor.installation_date;
            existingSensor.location_id = updatedSensor.location_id;

            await _context.SaveChangesAsync();
            return existingSensor;
        }

        public async Task DeleteSensorAsync(int sensorId)
        {
            var sensor = await _context.Sensors.FindAsync(sensorId);
            if (sensor == null)
            {
                throw new System.Exception("Сенсор не знайдено");
            }

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Отримує сенсори за вказаним статусом.
        /// </summary>
        /// <param name="status">Статус сенсора, за яким потрібно фільтрувати.</param>
        /// <returns>Список сенсорів, що відповідають заданому статусу.</returns>
        public async Task<List<Sensor>> GetSensorsByStatusAsync(string status)
        {
            string normalizedStatus = status.ToLower();
            return await _context.Sensors
                                 .Where(s => s.status.ToLower() == normalizedStatus)
                                 .Include(s => s.Location)
                                 .ToListAsync();
        }

        /// <summary>
        /// Отримує сенсори для вказаної локації.
        /// </summary>
        /// <param name="locationId">ID локації, для якої потрібно отримати сенсори.</param>
        /// <returns>Список сенсорів, які знаходяться на вказаній локації.</returns>
        public async Task<List<Sensor>> GetSensorsByLocationAsync(int locationId)
        {
            return await _context.Sensors
                                 .Where(s => s.location_id == locationId)
                                 .Include(s => s.Location)
                                 .ToListAsync();
        }
    }
}

