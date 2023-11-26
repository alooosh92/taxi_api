using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace taxi_api.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }        
        public int? Rating { get; set; } = 0;
        [Required]
        public IdentityUser? UserIdentity { get; set; }
    }
}
