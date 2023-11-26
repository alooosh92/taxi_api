using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using taxi_api.Models;

namespace taxi_api.VModels
{    
    public class ShowTripVM
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
        public string? UserName { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public double? UserRating { get; set; }
        [Required]
        public double Price { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        public DateTime? Accepted { get; set; }
        public DateTime? Ended { get; set; }
        [Required]
        public string? FirtName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? CarNumber { get; set; }
        [Required]
        public string? CarType { get; set; }
        [Required]
        public string? CarColor { get; set; }
    }
}
