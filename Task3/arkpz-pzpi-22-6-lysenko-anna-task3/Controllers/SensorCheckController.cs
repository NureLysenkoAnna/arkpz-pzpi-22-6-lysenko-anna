using GasDec.Models;
using GasDec.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GasDec.Controllers
{
    [ApiController]
    [Route("api/checks")]
    public class SensorCheckController : ControllerBase
    {
        private readonly SensorCheckService _sensorCheckService;

        public SensorCheckController(SensorCheckService sensorCheckService)
        {
            _sensorCheckService = sensorCheckService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати всі перевірки.")]
        public async Task<ActionResult<IEnumerable<SensorCheck>>> GetAllSensorChecks()
        {
            var sensorChecks = await _sensorCheckService.GetAllSensorChecksAsync();
            return Ok(sensorChecks);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати певну перевірку.")]
        public async Task<ActionResult<SensorCheck>> GetSensorCheckById(int id)
        {
            var sensorCheck = await _sensorCheckService.GetSensorCheckByIdAsync(id);
            if (sensorCheck == null)
            {
                return NotFound($"Запис перевірки з ID {id} не знайдено");
            }
            return Ok(sensorCheck);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Додати нову перевірку.")]
        public async Task<ActionResult<SensorCheck>> CreateSensorCheck([FromBody] SensorCheck sensorCheck)
        {
            var createdSensorCheck = await _sensorCheckService.CreateSensorCheckAsync(sensorCheck);
            return CreatedAtAction(nameof(GetSensorCheckById), 
                new { id = createdSensorCheck.check_id }, createdSensorCheck);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Оновити інформацію про перевірку сенсора.")]
        public async Task<IActionResult> UpdateSensorCheck(int id, [FromBody] SensorCheck updatedSensorCheck)
        {
            try
            {
                await _sensorCheckService.UpdateSensorCheckAsync(id, updatedSensorCheck);
                return Ok("Перевірку оновлено.");
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Видалити інформацію про перевірку сенсора.")]
        public async Task<IActionResult> DeleteSensorCheck(int id)
        {
            try
            {
                await _sensorCheckService.DeleteSensorCheckAsync(id);
                return Ok("Перевірку видалено.");
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("date/{date}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати всі перевірки за обраною датою.")]
        public async Task<ActionResult<IEnumerable<SensorCheck>>> GetSensorChecksByDate(string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return BadRequest("Некоректний формат дати");
            }

            var sensorChecks = await _sensorCheckService.GetSensorChecksByDateAsync(parsedDate);

            if (sensorChecks == null || sensorChecks.Count == 0)
            {
                return NotFound($"Перевірки на дату {date} не знайдені");
            }

            return Ok(sensorChecks);
        }

        [HttpGet("result/{result}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати всі перевірки з обраним результатом.")]
        public async Task<ActionResult<IEnumerable<SensorCheck>>> GetSensorChecksByResult(string result)
        {
            var sensorChecks = await _sensorCheckService.GetSensorChecksByResultAsync(result);

            if (sensorChecks == null || sensorChecks.Count == 0)
            {
                return NotFound($"Перевірки з результатом '{result}' не знайдені");
            }

            return Ok(sensorChecks);
        }

        [HttpGet("sensor/{sensorId}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати всі перевірки для обраного сенсора.")]
        public async Task<ActionResult<IEnumerable<SensorCheck>>> GetSensorChecksBySensorId(int sensorId)
        {
            var sensorChecks = await _sensorCheckService.GetSensorChecksBySensorIdAsync(sensorId);

            if (sensorChecks == null || sensorChecks.Count == 0)
            {
                return NotFound($"Перевірки для сенсора з ID {sensorId} не знайдено");
            }

            return Ok(sensorChecks);
        }
    }
}
