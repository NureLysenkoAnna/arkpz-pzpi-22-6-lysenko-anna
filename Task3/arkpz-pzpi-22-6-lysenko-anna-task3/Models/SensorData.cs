using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GasDec.Models;
using System.Text.Json.Serialization;

namespace GasDec.Models
{
    public class SensorData
    {
        [Key]
        public int data_id { get; set; }

        [Required]
        public double gas_level { get; set; }

        [Required]
        public double temperature { get; set; }

        [Required]
        public double pressure { get; set; }

        [Required]
        public DateTime time_stamp { get; set; }

        public int sensor_id { get; set; }

        [ForeignKey("sensor_id")]
        [JsonIgnore]
        public Sensor? Sensor { get; set; }
    }
}
