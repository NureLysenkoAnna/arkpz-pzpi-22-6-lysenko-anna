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
    public class Event
    {
        [Key]
        public int event_id { get; set; }

        [Required]
        public DateTime event_time { get; set; }

        [Required]
        [MaxLength(20)]
        public string severity { get; set; }

        public int data_id { get; set; }

        [ForeignKey("data_id")]
        [JsonIgnore]
        public SensorData? SensorData { get; set; }
    }
}
