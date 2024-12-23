using GasDec.Models;
using GasDec.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Manager")]
    [SwaggerOperation(Summary = "Отримати список всіх користувачів.")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати певного користувача.")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"Користувача з ID {id} не найдено." });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Під час обробки запиту сталася помилка.",
                details = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    [SwaggerOperation(Summary = "Створити нового користувача.")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user)
    {
        try
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUsers), new { id = createdUser.user_id }, createdUser);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    [SwaggerOperation(Summary = "Оновити обраного користувача.")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
    {
        try
        {
            await _userService.UpdateUserAsync(id, user);
            return Ok("Користувача оновлено успішно.");
        }
        catch (System.Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    [SwaggerOperation(Summary = "Видалити обраного користувача.")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok("Користувача видалено.");
        }
        catch (System.Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("location/{locationId}")]
    [Authorize(Roles = "Admin, Manager")]
    [SwaggerOperation(Summary = "Отримати користувачів за обраною локацією.")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersByLocation(int locationId)
    {
        var users = await _userService.GetUsersByLocationAsync(locationId);

        if (users == null || !users.Any())
        {
            return NotFound($"Користувачі для локації з ID {locationId} не знайдені.");
        }

        return Ok(users);
    }
}
