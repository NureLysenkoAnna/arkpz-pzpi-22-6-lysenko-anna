using GasDec.Models;
using GasDec.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GasDec.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class SensorDataController : ControllerBase
    {
        private readonly SensorDataService _sensorDataService;

        public SensorDataController(SensorDataService sensorDataService)
        {
            _sensorDataService = sensorDataService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати всі дані з сенсорів.")]
        public async Task<ActionResult<IEnumerable<SensorData>>> GetAllSensorData()
        {
            var data = await _sensorDataService.GetAllSensorDataAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати певні дані.")]
        public async Task<ActionResult<SensorData>> GetSensorDataById(int id)
        {
            var data = await _sensorDataService.GetSensorDataByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Дані з ID {id} не знайдено");
            }
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Створити нові дані.")]
        public async Task<ActionResult<SensorData>> CreateSensorData([FromBody] SensorData sensorData)
        {
            var createdData = await _sensorDataService.CreateSensorDataAsync(sensorData);
            return CreatedAtAction(nameof(GetSensorDataById), new { id = createdData.data_id }, createdData);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Оновити дані з сенсора.")]
        public async Task<IActionResult> UpdateSensorData(int id, [FromBody] SensorData updatedSensorData)
        {
            try
            {
                await _sensorDataService.UpdateSensorDataAsync(id, updatedSensorData);
                return Ok("Дані оновлено.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, LogicAdmin")]
        [SwaggerOperation(Summary = "Видалити дані, отримані з сенора.")]
        public async Task<IActionResult> DeleteSensorData(int id)
        {
            try
            {
                await _sensorDataService.DeleteSensorDataAsync(id);
                return Ok("Дані видалено.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Отримує ID останньо доданих даних сенсора.
        /// </summary>
        /// <returns>
        /// ID останніх даних сенсора або повідомлення про те, що дані відсутні.
        /// </returns>
        /// <response code="200">ID останніх даних сенсора успішно отримано.</response>
        /// <response code="404">Дані у таблиці SensorData відсутні.</response>
        [HttpGet("latestId")]
        [SwaggerOperation(Summary = "Отримати ID останньо доданих даних сенсора.\r\n.")]
        public async Task<IActionResult> GetLatestSensorDataId()
        {
            var latestSensorDataId = await _sensorDataService.GetLatestSensorDataIdAsync();

            if (latestSensorDataId == null)
            {
                return NotFound("Немає даних у таблиці SensorData.");
            }

            return Ok(latestSensorDataId);
        }
    }
}
