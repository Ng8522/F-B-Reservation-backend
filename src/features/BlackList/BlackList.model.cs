using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FnbReservationAPI.src.features.BlackList
{
    public class BlackList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "char(36)")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(20)]
        public required string ContactNumber { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Reason { get; set; }

        [Required]
        public required string Status { get; set; }

        public string CreateBy { get; set; } = "system";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UpdateBy { get; set; } = "system";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
