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
    public class User
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string user_name { get; set; }

        [Required]
        [MaxLength(30)]
        public string role { get; set; }

        [Required]
        [MaxLength(255)]
        public string password { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string email { get; set; }

        [MaxLength(15)]
        [Phone]
        public string? phone_number { get; set; }

        // Зовнішній ключ
        public int? location_id { get; set; }
        [ForeignKey("location_id")]

        [JsonIgnore]
        public Location? Location { get; set; }
    }
}
