﻿using System.ComponentModel.DataAnnotations;

namespace taxi_api.VModels
{
    public class ConnectWithUsVM
    {
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Body { get; set; }
    }
}
