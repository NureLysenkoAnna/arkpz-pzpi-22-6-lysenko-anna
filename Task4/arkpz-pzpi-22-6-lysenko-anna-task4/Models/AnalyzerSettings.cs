namespace IoTClient.Models
{
    /// <summary>
    /// Клас, що містить налаштування вагових коефіцієнтів для аналізу даних сенсора.
    /// </summary>
    public class AnalyzerSettings
    {
        public double GasWeight { get; set; }
        public double TemperatureWeight { get; set; }
        public double PressureWeight { get; set; }
    }
}
