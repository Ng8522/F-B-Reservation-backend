using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FnbReservationAPI.src.features.User
{
    public class UserDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? ContactNumber { get; set; }
        public required string Role { get; set; }
        public required string Status { get; set; }
        public string CreateBy { get; set; } = "system";        
        public DateTime CreatedAt { get; set; }
        public string UpdateBy { get; set; } = "system";
        public DateTime UpdatedAt { get; set; }
    }

}
