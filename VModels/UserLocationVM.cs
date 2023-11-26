using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class UserLocationVM
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public double? Late { get; set; }
        [Required]
        public double? Long { get; set; }
    }
}
