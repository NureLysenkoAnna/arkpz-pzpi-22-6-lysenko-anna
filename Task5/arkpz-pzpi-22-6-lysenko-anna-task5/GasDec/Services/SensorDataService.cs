using GasDec.Models;
using Microsoft.EntityFrameworkCore;

namespace GasDec.Services
{
    public class SensorDataService
    {
        private readonly GasLeakDbContext _context;

        public SensorDataService(GasLeakDbContext context)
        {
            _context = context;
        }

        public async Task<List<SensorData>> GetAllSensorDataAsync()
        {
            return await _context.SensorData
                                 .Include(sd => sd.Sensor)
                                 .ToListAsync();
        }

        public async Task<SensorData> GetSensorDataByIdAsync(int dataId)
        {
            return await _context.SensorData
                                 .Include(sd => sd.Sensor)
                                 .FirstOrDefaultAsync(sd => sd.data_id == dataId);
        }

        public async Task<SensorData> CreateSensorDataAsync(SensorData sensorData)
        {
            _context.SensorData.Add(sensorData);
            await _context.SaveChangesAsync();

            await MonitorNewSensorDataAsync(sensorData);

            return sensorData;
        }

        public async Task<SensorData> UpdateSensorDataAsync(int dataId, SensorData updatedSensorData)
        {
            var existingData = await _context.SensorData.FindAsync(dataId);
            if (existingData == null)
            {
                throw new Exception("Дані не знайдено");
            }

            existingData.gas_level = updatedSensorData.gas_level;
            existingData.temperature = updatedSensorData.temperature;
            existingData.pressure = updatedSensorData.pressure;
            existingData.time_stamp = updatedSensorData.time_stamp;
            existingData.sensor_id = updatedSensorData.sensor_id;

            await _context.SaveChangesAsync();
            return existingData;
        }

        public async Task DeleteSensorDataAsync(int dataId)
        {
            var sensorData = await _context.SensorData.FindAsync(dataId);
            if (sensorData == null)
            {
                throw new Exception("Дані не знайдено");
            }

            _context.SensorData.Remove(sensorData);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Отримання ID останнього запису з таблиці SensorData.
        /// </summary>
        /// <returns>
        /// ID останнього запису або null, якщо таблиця порожня.
        /// </returns>
        public async Task<int?> GetLatestSensorDataIdAsync()
        {
            return await _context.SensorData
                .OrderByDescending(sd => sd.time_stamp)
                .Select(sd => (int?)sd.data_id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Моніторинг нових даних сенсора та створення подій у випадку високого рівня небезпеки.
        /// </summary>
        /// <param name="sensorData">Дані сенсора, що містять показники газу, температури та тиску.</param>
        public async Task MonitorNewSensorDataAsync(SensorData sensorData)
        {
            double dangerLevel = CalculateDangerLevel(sensorData.gas_level, 
                sensorData.temperature, sensorData.pressure);

            if (dangerLevel > 10)
            {
                var eventSeverity = dangerLevel > 2 ? "High" : "Medium";
                var eventTime = DateTime.Now;

                var newEvent = new Event
                {
                    data_id = sensorData.data_id,
                    event_time = eventTime,
                    severity = SeverityLevel.High
                };

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                await SendNotificationToAdminAsync(newEvent, eventSeverity);
            }
        }

        /// <summary>
        /// Обчислення рівня небезпеки на основі показників сенсора.
        /// </summary>
        /// <param name="gasLevel">Рівень газу в ppm.</param>
        /// <param name="temperature">Температура в градусах Цельсія.</param>
        /// <param name="pressure">Тиск у барах.</param>
        /// <returns>Рівень небезпеки як числове значення.</returns>
        private double CalculateDangerLevel(double gasLevel, double temperature, double pressure)
        {
            double maxGasLevel = 50000;
            double maxTemperature = 30;
            double maxPressure = 10;

            return (gasLevel / maxGasLevel) + (temperature / maxTemperature) + (pressure / maxPressure);
        }

        /// <summary>
        /// Відправка сповіщення адміністратору та іншим одержувачам про нову подію.
        /// </summary>
        /// <param name="newEvent">Подія, створена на основі даних сенсора.</param>
        /// <param name="eventSeverity">Рівень серйозності події ("High" або "Medium").</param>
        public async Task SendNotificationToAdminAsync(Event newEvent, string eventSeverity)
        {
            var adminEmails = await _userService.GetAllAdminEmailsAsync();

            var sensorData = await _context.SensorData
                .Include(sd => sd.Sensor)
                .ThenInclude(s => s.Location)
                .FirstOrDefaultAsync(sd => sd.data_id == newEvent.data_id);

            if (sensorData == null)
            {
                Console.WriteLine("Для події не знайдено даних сенсора.");
                return;
            }

            var locationId = sensorData.Sensor?.location_id ?? 0;
            var residentEmails = await _userService.GetEmailsByLocationIdAsync(locationId);

            var managerEmails = await _userService.GetAllManagerEmailsAsync();

            var allRecipients = adminEmails
                .Concat(managerEmails)
                .Concat(residentEmails)
                .Distinct()
                .ToList();

            if (allRecipients.Count == 0)
            {
                Console.WriteLine("Одержувачів не знайдено.");
                return;
            }

            string locationName = sensorData.Sensor?.Location?.name ?? "невідомо";
            string locationFloor = sensorData.Sensor?.Location?.floor.ToString() ?? "невідомо";

            string message = $"Серйозність події: {eventSeverity}\n" +
                             $"Час фіксування: {newEvent.event_time}\n\n" +
                             $"Інформація, отримана з сенсора:\n" +
                             $"ID сенсора, що зафіксував подію: {sensorData.sensor_id}\n" +
                             $"Локація: {locationName}\n" +
                             $"Поверх: {locationFloor}\n" +
                             $"Показник вмісту газу: {sensorData.gas_level} ppm\n" +
                             $"Показник температури: {sensorData.temperature} °C\n" +
                             $"Показник тиску: {sensorData.pressure} bar\n\n" +
                             $"Будь ласка, негайно перевірте систему, щоб запобігти будь-яким небезпекам.";

            foreach (var recipientEmail in allRecipients)
            {
                await _emailService.SendEmailAsync(recipientEmail, "Увага! Зафіксовано аномальну подію!", message);
            }
        }
    }
}
