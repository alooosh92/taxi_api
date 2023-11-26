using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace taxi_api.Models
{
    public class ConnectWithUs
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public User? IdentityUser { get; set; }
        [Required]
        public string? Subject { get; set;}
        [Required]
        public string? Body { get; set; }
        [Required]
        public bool IsDone { get; set; } = false;

    }
}
