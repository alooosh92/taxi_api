using System.ComponentModel.DataAnnotations;

namespace taxi_api.Models
{
    public class TermsOfUseAndPrivacy
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Text { get; set; }
        [Required]
        public bool? isPrivacy { get; set; }
        
    }
}
