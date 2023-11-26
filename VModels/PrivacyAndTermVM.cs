using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class PrivacyAndTermVM
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Text { get; set; }
        [Required]
        public bool? IsPrivacy { get; set; }
    }
}
