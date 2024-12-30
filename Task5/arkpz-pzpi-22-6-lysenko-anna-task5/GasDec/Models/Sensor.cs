using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GasDec.Models
{
    public class Sensor
    {
        [Key]
        public int sensor_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string type { get; set; }

        [Required]
        [MaxLength(30)]
        public string status { get; set; }

        [Required]
        public DateTime installation_date { get; set; }

        public int location_id { get; set; }

        [ForeignKey("location_id")]
        [JsonIgnore]
        public Location? Location { get; set; }

        [JsonIgnore]
        public ICollection<SensorData>? SensorData { get; set; }

        [JsonIgnore]
        public ICollection<SensorCheck>? SensorChecks { get; set; }
    }
}
