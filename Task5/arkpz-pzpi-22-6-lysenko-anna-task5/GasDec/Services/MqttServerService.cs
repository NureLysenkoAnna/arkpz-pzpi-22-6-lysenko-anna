using GasDec.Models;
using MQTTnet;
using MQTTnet.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class MqttService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MqttService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Запускає MQTT-клієнта для підключення до брокера, підписки на топіки та обробки отриманих повідомлень.
    /// </summary>
    /// <remarks>
    /// Клієнт підключається до брокера MQTT (broker.hivemq.com) та підписується на два топіки:
    /// 1. iot/gasDec/sensors/data - для отримання даних сенсорів.
    /// 2. iot/gasDec/sensors/events - для отримання подій.
    /// Оброблені повідомлення зберігаються у базі даних за допомогою DbContext.
    /// </remarks>
    /// <exception cref="Exception">Виникає у разі помилки підключення до брокера або роботи з БД.</exception>
    /// <returns>Завдання, яке представляє асинхронну операцію.</returns>

    public async Task StartAsync()
    {
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();

        var mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("broker.hivemq.com", 1883)
            .Build();

        // Обробка отриманих повідомлень
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"Отримано данi: {payload}");

            if (e.ApplicationMessage.Topic == "iot/gasDec/sensors/data")
            {
                // Десеріалізація даних
                var sensorData = JsonSerializer.Deserialize<SensorData>(payload);
                if (sensorData != null)
                {
                    try
                    {
                        // Створення нового обсягу для роботи з DbContext
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<GasLeakDbContext>();
                            await context.SensorData.AddAsync(sensorData);
                            await context.SaveChangesAsync();
                            Console.WriteLine("Данi успiшно збережено у БД.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка збереження в БД: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Не вдалося десерiалiзувати повiдомлення.");
                }
            }
            else if (e.ApplicationMessage.Topic == "iot/gasDec/sensors/events")
            {
                // Десеріалізація події
                var eventData = JsonSerializer.Deserialize<Event>(payload);
                if (eventData != null)
                {
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<GasLeakDbContext>();
                            await context.Events.AddAsync(eventData);
                            await context.SaveChangesAsync();
                            Console.WriteLine("Подiя успiшно збережена у БД.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка збереження події в БД: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Не вдалося десерiалізувати подiю.");
                }
            }
        };

        // Обробка підключення
        mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("Пiдключено до брокера.");
            await mqttClient.SubscribeAsync("iot/gasDec/sensors/data");
            await mqttClient.SubscribeAsync("iot/gasDec/sensors/events");
            Console.WriteLine("Пiдписано на топiки: iot/gasDec/sensors/data, iot/gasDec/sensors/events");
        };

        // Обробка відключення
        mqttClient.DisconnectedAsync += e =>
        {
            Console.WriteLine("Вiдключено вiд брокера.");
            return Task.CompletedTask;
        };

        // Підключення до брокера
        try
        {
            await mqttClient.ConnectAsync(mqttOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка пiдключення: {ex.Message}");
        }
    }
}
