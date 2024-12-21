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
    public class Notification
    {
        [Key]
        public int notification_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string message { get; set; }

        [Required]
        public DateTime formation_time { get; set; }

        [Required]
        [MaxLength(30)]
        public string notification_type { get; set; }

        public int user_id { get; set; }

        [ForeignKey("user_id")]
        [JsonIgnore]
        public User? User { get; set; }

        public int event_id { get; set; }

        [ForeignKey("event_id")]
        [JsonIgnore]
        public Event? Event { get; set; }
    }
}
