using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace taxi_api.Models
{
    public class Trip
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public double? FromLate { get; set; }
        [Required]
        public double? FromLong { get; set; }
        [Required]
        public double? ToLate { get; set; }
        [Required]
        public double? ToLong { get; set; }
        [Required]
        public User? User { get; set; }
        [Required]
        public double Price { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        public DateTime? Accepted { get; set; }
        public DateTime? Ended { get; set; }
        public Driver? Drive { get; set; }
    }
}
