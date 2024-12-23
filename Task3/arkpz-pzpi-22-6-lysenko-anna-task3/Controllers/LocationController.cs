using GasDec.Models;
using GasDec.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GasDec.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Отримати всі локації.
        /// </summary>
        /// <returns>Список всіх локацій.</returns>
        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Отримати всі локації.")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        /// <summary>
        /// Знайти локацію за ID.
        /// </summary>
        /// <param name="id">ID локації для пошуку.</param>
        /// <returns>Локація з відповідним ID, або NotFound якщо локація не знайдена.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Знайти локацію за id.")]
        public async Task<ActionResult<Location>> GetLocationById(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null)
            {
                return NotFound($"Локація з ID {id} не знайдена");
            }
            return Ok(location);
        }

        /// <summary>
        /// Створити нову локацію.
        /// </summary>
        /// <param name="location">Дані нової локації.</param>
        /// <returns>CreatedAtAction з новоствореною локацією.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Створити нову локацію.")]
        public async Task<IActionResult> AddLocation([FromBody] Location location)
        {
            if (location == null)
            {
                return BadRequest("Invalid location data.");
            }

            location.location_id = 0;

            var addedLocation = await _locationService.AddLocationAsync(location);
            return CreatedAtAction(nameof(GetLocationById), new { id = addedLocation.location_id }, addedLocation);
        }

        /// <summary>
        /// Оновити обрану локацію.
        /// </summary>
        /// <param name="id">ID локації, яку потрібно оновити.</param>
        /// <param name="updatedLocation">Оновлені дані локації.</param>
        /// <returns>Ok з повідомленням про успішне оновлення або NotFound якщо локація не знайдена.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Оновити обрану локацію.")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location updatedLocation)
        {
            try
            {
                await _locationService.UpdateLocationAsync(id, updatedLocation);
                return Ok("Інформацію оновлено.");
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Видалити обрану локацію.
        /// </summary>
        /// <param name="id">ID локації, яку потрібно видалити.</param>
        /// <returns>Ok з повідомленням про успішне видалення або NotFound якщо локація не знайдена.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Видалити обрану локацію.")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                await _locationService.DeleteLocationAsync(id);
                return Ok("Локацію видалено.");
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Отримати локації за обраним поверхом.
        /// </summary>
        /// <param name="floor">Номер поверху для пошуку локацій.</param>
        /// <returns>Список локацій на зазначеному поверсі або NotFound, якщо такі локації відсутні.</returns>
        [HttpGet("floor/{floor}")]
        [Authorize]
        [SwaggerOperation(Summary = "Отримати локації за обраним поверхом.")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocationsByFloor(int floor)
        {
            var locations = await _locationService.GetLocationsByFloorAsync(floor);

            if (locations == null || !locations.Any())
            {
                return NotFound($"Локації на поверсі {floor} не знайдені.");
            }

            return Ok(locations);
        }

        /// <summary>
        /// Отримати локації за обраним типом.
        /// </summary>
        /// <param name="type">Тип локації для пошуку.</param>
        /// <returns>Список локацій з відповідним типом або NotFound, якщо такі локації відсутні.</returns>
        [HttpGet("type/{type}")]
        [Authorize(Roles = "Admin, LogicAdmin, Manager")]
        [SwaggerOperation(Summary = "Отримати локації за обраним типом.")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocationsByType(string type)
        {
            var locations = await _locationService.GetLocationsByTypeAsync(type);

            if (locations == null || !locations.Any())
            {
                return NotFound($"Локації з типом '{type}' не знайдені.");
            }

            return Ok(locations);
        }

        /// <summary>
        /// Отримати мінімальну кількість сенсорів для обраної локації.
        /// </summary>
        /// <param name="locationId">ID локації для обчислення необхідної кількості сенсорів.</param>
        /// <param name="minRequiredSensors">Мінімальна кількість сенсорів.</param>
        /// <returns>Кількість необхідних сенсорів.</returns>
        [HttpGet("calculate_sensors/{locationId}")]
        [Authorize(Roles = "LogicAdmin")]
        [SwaggerOperation(Summary = "Отримати мінімальну кількість сенсорів для обраної локації.")]
        public async Task<IActionResult> CalculateSensors(int locationId, [FromQuery] int? minRequiredSensors = null)
        {
            try
            {
                var requiredSensors = await _locationService.CalculateRequiredSensorsAsync(locationId, minRequiredSensors);
                return Ok(new { RequiredSensors = requiredSensors });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
