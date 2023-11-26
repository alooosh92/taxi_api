using System.ComponentModel.DataAnnotations;

namespace taxi_api.Models
{
    public class StaticValue
    {
        [Key]
        public string? Key { get; set; }
        [Required]
        public string? Value { get; set; }
    }
}
