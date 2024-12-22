using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GasDec.Models;

namespace GasDec.Models
{
    public class Location
    {
        [Key]
        public int location_id { get; set; }

        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        [Required]
        [MaxLength(50)]
        public string location_type { get; set; }

        [Required]
        public int floor { get; set; }

        public double? area { get; set; }

        [JsonIgnore]
        public ICollection<User>? Users { get; set; }

        [JsonIgnore]
        public ICollection<Sensor>? Sensors { get; set; }
    }
}
