using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class RegisterVM
    {
        [Required]
        [Phone]
        public string? Phone { get; set; }
        [EmailAddress]
        public string? Email { get; set;}
        [Required] 
        public string? Name { get; set;}
    }
}
