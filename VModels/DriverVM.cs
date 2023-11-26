using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class DriverVM
    {
        [Required]
        public string? FirtName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? FatherName { get; set; }
        [Required]
        public string? LicenType { get; set; }
        [Required]
        public string? CarNumber { get; set; }
        [Required]
        public string? CarType { get; set; }
        [Required]
        public string? CarColor { get; set; }
        [Required]
        public double? Balance { get; set; }
        [Required]
        public string? Phone { get;set; }
    }
}
