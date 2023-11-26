using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class ShowUserLocationVM
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        [Required]
        public double? Late { get; set; }
        [Required]
        public double? Long { get; set; }
    }
}
