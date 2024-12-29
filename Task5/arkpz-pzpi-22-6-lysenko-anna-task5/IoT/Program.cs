using IoTClient.Models;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using IoTClient.Services;
using Microsoft.Extensions.Configuration;
using IoTClient.Utilities;

class Program
{
    static async Task Main(string[] args)
    {
        // Завантаження налаштувань з конфігураційного файлу.
        var config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        var settings = config.GetSection("AnalyzerSettings").Get<AnalyzerSettings>();

        // Ініціалізація налаштувань
        SensorDataAnalyzer.InitializeSettings(settings);

        // Завантаження MQTT-параметрів з конфігурації
        var mqttSettings = config.GetSection("MqttSettings");
        string broker = mqttSettings["Broker"];
        int port = int.Parse(mqttSettings["Port"]);
        string sensorDataTopic = mqttSettings.GetSection("Topics")["SensorData"];
        string sensorEventTopic = mqttSettings.GetSection("Topics")["SensorEvent"];
        int sendInterval = int.Parse(mqttSettings["SendIntervalMs"]);

        var httpClient = new HttpClient();
        var sensorService = new SensorService(httpClient);

        // Отримуємо sensor_id активних сенсорів
        var activeSensorIds = await sensorService.GetActiveSensorIdsAsync();

        // Ініціалізація MQTT-клієнта
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();

        var mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(broker, port)
            .Build();

        // Обробка події успішного підключення до брокера
        mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("Пiдключено до брокера.");
            
            while (true)
            {   
                var activeSensorIds = await sensorService.GetActiveSensorIdsAsync();

                foreach (var sensorId in activeSensorIds)
                {
                    var sensorData = new
                    {
                        sensor_id = sensorId,
                        gas_level = SensorSimulator.GenerateGasLevel(),
                        temperature = SensorSimulator.GenerateTemperature(),
                        pressure = SensorSimulator.GeneratePressure(),
                        time_stamp = DateTime.UtcNow
                    };

                    string payload = JsonSerializer.Serialize(sensorData);

                    // Формування MQTT-повідомлення для передачі даних сенсора.
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(sensorDataTopic)
                        .WithPayload(payload)
                        .WithQualityOfServiceLevel(MQTTnet.Protocol
                        .MqttQualityOfServiceLevel.ExactlyOnce)
                        .WithRetainFlag(false)
                        .Build();

                    // Публікація повідомлення.
                    await mqttClient.PublishAsync(message);
                    Console.WriteLine($"Данi для сенсора {sensorId} надiслано: {payload}");

                    var severity = SensorDataAnalyzer.AnalyzeSensorData(sensorData);
                    
                    // Отримуємо останнє data_id
                    var dataId = await sensorService.GetLatestDataIdAsync();

                    if (severity != SeverityLevel.Low)
                    {
                        var newEvent = new
                        {
                            event_time = DateTime.UtcNow,
                            severity = severity,
                            data_id = dataId.Value
                        };

                        string eventPayload = JsonSerializer.Serialize(newEvent);

                        // Формування MQTT-повідомлення для події.
                        var eventMessage = new MqttApplicationMessageBuilder()
                            .WithTopic(sensorEventTopic)
                            .WithPayload(eventPayload)
                            .WithQualityOfServiceLevel(MQTTnet.Protocol
                            .MqttQualityOfServiceLevel.ExactlyOnce)
                            .WithRetainFlag(false)
                            .Build();

                        // Публікація повідомлення події.
                        await mqttClient.PublishAsync(eventMessage);
                        Console.WriteLine($"Зафiксовано аномалiю. Подiя передана: {eventPayload}");
                    }
                    else
                    {
                        Console.WriteLine("Данi не викликають серйозну подiю або аномалiю подiї.");
                    }
                }

                await Task.Delay(sendInterval);
            }
        };

        // Обробка події відключення від брокера
        mqttClient.DisconnectedAsync += async e =>
        {
            Console.WriteLine("Вiдключено вiд брокера.");
            await Task.Delay(TimeSpan.FromSeconds(5));
            try
            {
                await mqttClient.ConnectAsync(mqttOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка перепiдключення: {ex.Message}");
            }
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

        Console.ReadLine();
    }
}

