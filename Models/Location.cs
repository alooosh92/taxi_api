using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace taxi_api.Models
{
    public class Location
    {
        [Key]        
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        [Required]
        public double? Late { get; set; }
        [Required]
        public double? Long { get; set; }
        [Required]
        public User? User { get; set; }

    }
}
