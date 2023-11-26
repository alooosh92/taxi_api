using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class VirefyVM
    {
        [Required]
        [Phone]
        public string? Phone { get; set; }
        [Required]
        public int Code { get; set; }
    }
}
