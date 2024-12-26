using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IoTClient.Models;
using System.Collections.Generic;
using System;

/// <summary>
/// Клас для роботи з сервісами сенсорів через HTTP-запити.
/// </summary>
public class SensorService
{
    private readonly HttpClient _httpClient;

    public SensorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Асинхронний метод для отримання ідентифікаторів сенсорів, що мають статус "activate".
    /// </summary>
    /// <returns>Список ідентифікаторів активних сенсорів або порожній список у разі помилки.</returns>
    public async Task<List<int>> GetActiveSensorIdsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:5199/api/sensors/byStatus/activate");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Перевірка на правильність типу даних
                if (response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    var sensorIds = JsonSerializer.Deserialize<List<int>>(content);
                    return sensorIds ?? new List<int>();
                }
                else
                {
                    Console.WriteLine("Неправильний формат вiдповiдi, очiкується JSON.");
                    return new List<int>();
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Помилка при отриманнi сенсорiв: {response.StatusCode}," +
                    $" Тiло: {errorContent}");
                return new List<int>();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Помилка при запитi: {ex.Message}");
            return new List<int>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Неочiкувана помилка: {ex.Message}");
            return new List<int>();
        }
    }

    /// <summary>
    /// Асинхронний метод для отримання ідентифікатора останньо доданих даних сенсора.
    /// </summary>
    /// <returns>Останній ідентифікатор даних сенсора або null у разі помилки.</returns>
    public async Task<int?> GetLatestDataIdAsync()
    {
        var response = await _httpClient.GetAsync("http://localhost:5199/api/data/latestId");

        var responseContent = await response.Content.ReadAsStringAsync();
        if (int.TryParse(responseContent, out var latestSensorDataId))
        {
            return latestSensorDataId;
        }
        else
        {
            Console.WriteLine("Помилка: не вдалося конвертувати вiдповiдь у число.");
            return null;
        }
    }
}
