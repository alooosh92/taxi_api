using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class TripForWebSocketVW
    {
        [Required]
        public Guid? Id { get; set; }
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
        [Required]
        public double Distance { get; set; }
        [Required]
        public string? End { get; set; }
        [Required]
        public string? Start { get; set; }
        [Required]
        public bool IsAccepted { get; set; }
    }
}
