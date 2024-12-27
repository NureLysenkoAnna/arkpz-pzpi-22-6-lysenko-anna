using IoTClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTClient.Services
{
    /// <summary>
    /// Статичний клас для аналізу даних сенсорів та визначення рівня загрози.
    /// </summary>
    public static class SensorDataAnalyzer
    {
        private static AnalyzerSettings _settings;

        public static void InitializeSettings(AnalyzerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Аналізує дані сенсора та визначає рівень загрози на основі заданих ваг та нормалізації параметрів.
        /// </summary>
        /// <param name="sensorData">Дані сенсора, що містять рівень газу, температуру та тиск.</param>
        /// <returns>Рівень загрози (Low, Medium, High).</returns>
        public static SeverityLevel AnalyzeSensorData(dynamic sensorData)
        {
            // Визначаємо коефіцієнти ваги для кожного параметра
            double gasWeight = _settings.GasWeight;
            double temperatureWeight = _settings.TemperatureWeight;
            double pressureWeight = _settings.PressureWeight;

            // Отримуємо значення з даних сенсора
            double gasLevel = sensorData.gas_level;
            double temperature = sensorData.temperature;
            double pressure = sensorData.pressure;

            // Нормалізація значень
            double normalizedGas = Normalize(gasLevel, 0, 100);
            double normalizedTemperature = Normalize(temperature, 0, 50);
            double normalizedPressure = Normalize(pressure, 1, 10);

            // Розраховуємо загальний бал на основі нормалізованих значень та ваг
            double totalScore = (normalizedGas * gasWeight) + (normalizedTemperature * temperatureWeight)
                + (normalizedPressure * pressureWeight);

            // Визначаємо рівень загрози на основі балу
            if (totalScore > 0.75)
            {
                return SeverityLevel.High;
            }
            else if (totalScore > 0.5)
            {
                return SeverityLevel.Medium;
            }
            else
            {
                return SeverityLevel.Low;
            }
        }

        /// <summary>
        /// Нормалізує значення в заданому діапазоні.
        /// </summary>
        /// <param name="value">Поточне значення.</param>
        /// <param name="minValue">Мінімальне можливе значення.</param>
        /// <param name="maxValue">Максимальне можливе значення.</param>
        /// <returns>Нормалізоване значення в межах від 0 до 1.</returns>
        private static double Normalize(double value, double minValue, double maxValue)
        {
            return (value - minValue) / (maxValue - minValue);
        }
    }
}
