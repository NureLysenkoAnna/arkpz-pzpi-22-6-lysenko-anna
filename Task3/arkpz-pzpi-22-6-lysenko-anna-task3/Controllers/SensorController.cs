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
    }
}
