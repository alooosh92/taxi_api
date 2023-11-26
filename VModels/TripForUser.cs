using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class TripForUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public double Price { get; set; }
        [Required] 
        public double Distance { get; set; }
        [Required]
        public string? End { get; set; }
        [Required]
        public string? Start { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Accepted { get; set; }
        public DateTime? Ended { get; set; }
        [Required]
        public string? DriverName { get; set; }
        [Required]
        public string? CarNumber { get; set; }
        [Required]
        public string? CarType { get; set; }
        [Required]
        public string? CarColor { get; set; }
    }
}
