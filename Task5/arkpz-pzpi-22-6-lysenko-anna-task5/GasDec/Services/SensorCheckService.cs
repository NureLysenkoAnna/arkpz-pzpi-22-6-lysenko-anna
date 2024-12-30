using GasDec.Models;
using Microsoft.EntityFrameworkCore;

namespace GasDec.Services
{
    public class SensorCheckService
    {
        private readonly GasLeakDbContext _context;

        public SensorCheckService(GasLeakDbContext context)
        {
            _context = context;
        }

        public async Task<List<SensorCheck>> GetAllSensorChecksAsync()
        {
            return await _context.SensorChecks
                                 .Include(sc => sc.Sensor)
                                 .ToListAsync();
        }

        public async Task<SensorCheck> GetSensorCheckByIdAsync(int checkId)
        {
            return await _context.SensorChecks
                                 .Include(sc => sc.Sensor)
                                 .FirstOrDefaultAsync(sc => sc.check_id == checkId);
        }

        public async Task<SensorCheck> CreateSensorCheckAsync(SensorCheck sensorCheck)
        {
            _context.SensorChecks.Add(sensorCheck);
            await _context.SaveChangesAsync();

            if (sensorCheck.result.ToLower() == "failed")
            {
                await SendNotificationToManagerAsync(sensorCheck);
            }
            
            return sensorCheck;
        }

        public async Task<SensorCheck> UpdateSensorCheckAsync(int checkId, SensorCheck updatedSensorCheck)
        {
            var existingCheck = await _context.SensorChecks.FindAsync(checkId);
            if (existingCheck == null)
            {
                throw new System.Exception("Запис перевірки не знайдено");
            }

            existingCheck.check_date = updatedSensorCheck.check_date;
            existingCheck.result = updatedSensorCheck.result;
            existingCheck.sensor_id = updatedSensorCheck.sensor_id;

            await _context.SaveChangesAsync();
            return existingCheck;
        }

        public async Task DeleteSensorCheckAsync(int checkId)
        {
            var sensorCheck = await _context.SensorChecks.FindAsync(checkId);
            if (sensorCheck == null)
            {
                throw new System.Exception("Запис перевірки не знайдено");
            }

            _context.SensorChecks.Remove(sensorCheck);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Отримує всі перевірки сенсорів за вказаною датою.
        /// </summary>
        /// <param name="date">Дата перевірки для фільтрації.</param>
        /// <returns>Список перевірок, що відповідають вказаній даті.</returns>
        public async Task<List<SensorCheck>> GetSensorChecksByDateAsync(DateTime date)
        {
            return await _context.SensorChecks
                                 .Where(sc => sc.check_date == date)
                                 .Include(sc => sc.Sensor)
                                 .ToListAsync();
        }

        /// <summary>
        /// Отримує всі перевірки сенсорів за вказаним результатом перевірки.
        /// </summary>
        /// <param name="result">Результат перевірки для фільтрації.</param>
        /// <returns>Список перевірок, що відповідають вказаному результату.</returns>
        public async Task<List<SensorCheck>> GetSensorChecksByResultAsync(string result)
        {
            string normalizedResult = result.ToLower();
            return await _context.SensorChecks
                                 .Where(sc => sc.result.ToLower() == normalizedResult)
                                 .Include(sc => sc.Sensor)
                                 .ToListAsync();
        }

        /// <summary>
        /// Отримує всі перевірки для конкретного сенсора за його ID.
        /// </summary>
        /// <param name="sensorId">ID сенсора для фільтрації перевірок.</param>
        /// <returns>Список перевірок для вказаного сенсора.</returns>
        public async Task<List<SensorCheck>> GetSensorChecksBySensorIdAsync(int sensorId)
        {
            return await _context.SensorChecks
                                 .Where(sc => sc.sensor_id == sensorId)
                                 .Include(sc => sc.Sensor)
                                 .ToListAsync();
        }

        /// <summary>
        /// Надсилає сповіщення менеджеру про те, що сенсор не пройшов технічну перевірку.
        /// </summary>
        /// <param name="sensorCheck">Об'єкт, що містить інформацію про перевірку сенсора.</param>
        private async Task SendNotificationToManagerAsync(SensorCheck sensorCheck)
        {
            var managerEmail = await _context.Users
                .Where(u => u.role == "Manager")
                .Select(u => u.email)
                .FirstOrDefaultAsync();

            var sensor = await _context.Sensors
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.sensor_id == sensorCheck.sensor_id);

            if (sensor != null && sensor.Location != null)
            {
                var locationName = sensor.Location.name ?? "невідомо";
                var locationFloor = sensor.Location.floor.ToString() ?? "невідомо";

                var subject = "Сенсор не пройшов технічну перевірку";
                var body = $"Сенсор з ID {sensorCheck.sensor_id} не пройшов технічну перевірку та потребує оновлення.\n" +
                        $"Розташування сенсора:\n" +
                        $"- Локація: {locationName}\n" +
                        $"- Поверх: {locationFloor}";

                await _emailService.SendEmailAsync(managerEmail, subject, body);
            }
            else
            {
                Console.WriteLine("Не вдалося знайти сенсор або дані про його локацію.");
            }
        }
    }
}

