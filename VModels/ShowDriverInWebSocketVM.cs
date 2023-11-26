using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class ShowDriverInWebSocketVM
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? CarNumber { get; set; }
        [Required]
        public string? CarType { get; set; }
        [Required]
        public string? CarColor { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? PhonetripAccepted { get; set;}
        [Required]
        public bool? IsEmpty { get; set; }
    }
}
