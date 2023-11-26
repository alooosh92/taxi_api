using System.ComponentModel.DataAnnotations;

namespace taxi_api.Models
{
    public class VirefyCode
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MinLength(12)]
        public string? Phone { get; set; }
        [MinLength(4)]
        [MaxLength(4)]
        [Required]
        public int Code { get; set; } = 1234;//new Random().Next(1000, 9999);
        [Required]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}
