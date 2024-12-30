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
    public enum SeverityLevel
    {
        Low = 1,    
        Medium = 2,
        High = 3
    }

    public class Event
    {
        [Key]
        public int event_id { get; set; }

        [Required]
        public DateTime event_time { get; set; }

        [Required]
        public SeverityLevel severity { get; set; }

        public int data_id { get; set; }

        [ForeignKey("data_id")]
        [JsonIgnore]
        public SensorData? SensorData { get; set; }
    }
}
