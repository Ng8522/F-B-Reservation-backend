using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FnbReservationAPI.src.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "char(36)")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [AllowNull]
        [MaxLength(20)]
        public string ContactNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; } // "Admin", "Outlet_Staff", "Customer"

        [Required]
        public required string Status { get; set; }

        public string CreateBy { get; set; } = "system";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UpdateBy { get; set; } = "system";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}