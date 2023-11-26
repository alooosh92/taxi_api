using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class LoginVM
    {
        [Required]
        [Phone]
        public string? Phone { get; set; }
    }
}
