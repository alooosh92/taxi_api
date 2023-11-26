using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace taxi_api.Models
{
    public class Driver
    {
        [Key]
        public Guid Id {  get; set; } = Guid.NewGuid();
        [Required]
        public string? FirtName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? FatherName { get; set; }
        [Required]
        public string? LicenType { get; set;}
        [Required] 
        public string? CarNumber { get; set; }
        [Required]
        public string? CarType { get; set;}
        [Required]
        public string? CarColor { get; set;}
        [Required]
        public double? Balance { get; set;}
        [Required]        
        public bool? IsEmpty { get; set; } = false;
        [Required]
        public DateTime? Created { get; set; } = DateTime.Now;
        [Required] 
        public IdentityUser? IdentityUser { get; set; } 
    }
}
