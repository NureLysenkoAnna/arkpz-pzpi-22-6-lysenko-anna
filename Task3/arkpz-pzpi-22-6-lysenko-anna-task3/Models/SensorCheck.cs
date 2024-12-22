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
    public class SensorCheck
    {
        [Key]
        public int check_id { get; set; }

        [Required]
        public DateTime check_date { get; set; }

        [Required]
        [MaxLength(50)]
        public string result { get; set; }

        public int sensor_id { get; set; }

        [ForeignKey("sensor_id")]
        [JsonIgnore]
        public Sensor? Sensor { get; set; }
    }
}
