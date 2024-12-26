using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTClient.Utilities
{
    /// <summary>
    /// Статичний клас для імітації роботи сенсорів шляхом генерації випадкових даних.
    /// </summary>
    public static class SensorSimulator
    {
        private static Random _random = new Random();

        public static double GenerateGasLevel() => Math.Round(_random.NextDouble() * 100, 2);
        public static double GenerateTemperature() => Math.Round(_random.NextDouble() * 50, 2);
        public static double GeneratePressure() => Math.Round(_random.NextDouble() * 10 + 1, 2);
    }
}
