﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace WorkshopsGov.Models.DTOs
{
    public class UserRegistrationRequestDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
