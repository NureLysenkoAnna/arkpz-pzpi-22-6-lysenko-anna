using GasDec.Models;
using GasDec.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GasDec.Controllers
{
    [ApiController]
    [Route("api/sensors")]
    public class SensorController : ControllerBase
    {
        private readonly SensorService _sensorService;

        public SensorController(SensorService sensorService)
        {
            _sensorService = sensorService;
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Отримати всі сенсори.")]
        public async Task<ActionResult<IEnumerable<Sensor>>> GetAllSensors()
        {
            var sensors = await _sensorService.GetAllSensorsAsync();
            return Ok(sensors);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Отримати певний сенсор.")]
        public async Task<ActionResult<Sensor>> GetSensorById(int id)
        {
            var sensor = await _sensorService.GetSensorByIdAsync(id);
            if (sensor == null)
            {
                return NotFound($"Сенсор з ID {id} не знайдено");
            }
            return Ok(sensor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Додати новий сенсор.")]
        public async Task<ActionResult<Sensor>> CreateSensor([FromBody] Sensor sensor)
        {
            var createdSensor = await _sensorService.CreateSensorAsync(sensor);
            return CreatedAtAction(nameof(GetSensorById), 
                new { id = createdSensor.sensor_id }, createdSensor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Оновити обраний сенсор.")]
        public async Task<IActionResult> UpdateSensor(int id, [FromBody] Sensor updatedSensor)
        {
            try
            {
                await _sensorService.UpdateSensorAsync(id, updatedSensor);
                return Ok("Сенсор оновлено.");
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Видалити обраний сенсор.")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            try
            {
                await _sensorService.DeleteSensorAsync(id);
                return Ok("Сенсор видалено.");
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Отримати сенсори з обраним статусом.
        /// </summary>
        /// <param name="status">Статус сенсорів для фільтрації (наприклад, 'Active', 'Inactive').</param>
        /// <returns>Список сенсорів з відповідним статусом або NotFound, якщо сенсори з таким статусом не знайдені.</returns>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати сенсори з обраним статусом.")]
        public async Task<ActionResult<IEnumerable<Sensor>>> GetSensorsByStatus(string status)
        {
            var sensors = await _sensorService.GetSensorsByStatusAsync(status);

            if (sensors == null || sensors.Count == 0)
            {
                return NotFound($"Сенсори зі статусом '{status}' не знайдено.");
            }

            return Ok(sensors);
        }


        /// <summary>
        /// Отримує список ID сенсорів з обраним статусом.
        /// </summary>
        /// <param name="status">Статус сенсорів, які потрібно знайти.</param>
        /// <returns>
        /// Список ID сенсорів із заданим статусом або повідомлення про те, що сенсори не знайдено.
        /// </returns>
        /// <response code="200">Список ID сенсорів успішно отримано.</response>
        /// <response code="404">Сенсори зі статусом не знайдено.</response>
        [HttpGet("byStatus/{status}")]
        [SwaggerOperation(Summary = "Отримати ID сенсорів з обраним статусом.")]
        public async Task<ActionResult<IEnumerable<int>>> GetSensorIdsByStatus(string status)
        {
            var sensorIds = await _sensorService.GetSensorIdsByStatusAsync(status);

            if (sensorIds == null || sensorIds.Count == 0)
            {
                return NotFound($"Сенсори зі статусом '{status}' не знайдено.");
            }

            return Ok(sensorIds);
        }

        /// <summary>
        /// Отримати сенсори на вказаній локації.
        /// </summary>
        /// <param name="locationId">ID локації для фільтрації сенсорів.</param>
        /// <returns>Список сенсорів на вказаній локації або NotFound, якщо сенсори в цій локації не знайдені.</returns>
        [HttpGet("location/{locationId}")]
        [Authorize(Roles = "Admin, Manager")]
        [SwaggerOperation(Summary = "Отримати сенсори на вказаній локації.")]
        public async Task<ActionResult<IEnumerable<Sensor>>> GetSensorsByLocation(int locationId)
        {
            var sensors = await _sensorService.GetSensorsByLocationAsync(locationId);

            if (sensors == null || sensors.Count == 0)
            {
                return NotFound($"Сенсори в локації з ID {locationId} не знайдено.");
            }

            return Ok(sensors);
        }

        /// <summary>
        /// Перевірка потреби оновлень сенсорів.
        /// </summary>
        /// <param name="sensorId">ID сенсора, для якого перевіряється стан.</param>
        /// <param name="recommendedLifetime">Рекомендований термін служби сенсора для перевірки необхідності оновлення.</param>
        /// <returns>Статус потреби оновлення сенсора.</returns>
        [HttpGet("outdated_check/{sensorId}")]
        [Authorize(Roles = "LogicAdmin")]
        [SwaggerOperation(Summary = "Перевірка потреби оновлень сенсорів.")]
        public async Task<IActionResult> GetSensorStatus(int sensorId, [FromQuery] int recommendedLifetime)
        {
            var status = await _sensorService.CalculateSensorLifespanAndCheck(sensorId, recommendedLifetime);
            return Ok(status);
        }       
    }
}
