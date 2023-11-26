using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using taxi_api.Models;

namespace taxi_api.VModels
{
    public class TripVM
    {
        [Required]
        public double? FromLate { get; set; }
        [Required]
        public double? FromLong { get; set; }
        [Required]
        public double? ToLate { get; set; }
        [Required]
        public double? ToLong { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
