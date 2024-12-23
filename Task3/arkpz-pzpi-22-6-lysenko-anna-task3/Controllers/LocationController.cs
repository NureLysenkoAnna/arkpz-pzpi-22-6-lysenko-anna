﻿using GasDec.Models;
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

        [HttpGet]
        [SwaggerOperation(Summary = "Отримати всі локації.")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        [HttpGet("{id}")]
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

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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

        [HttpGet("floor/{floor}")]
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

        [HttpGet("type/{type}")]
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
    }
}