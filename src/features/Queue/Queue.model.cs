using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FnbReservationAPI.src.features.Queue
{
    public class Queue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "char(36)")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column(TypeName = "char(36)")]
        public Guid CustomerId { get; set; }

        [Required]
        [Column(TypeName = "char(36)")]
        public Guid OutletId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int QueuePosition { get; set; }      

        [Required]
        public int NumberOfGuests { get; set;}   

        [AllowNull]
        [MaxLength(255)]
        public required string SpecialRequests { get; set; }

        [Required]
        public required string Status { get; set; }

        public string CreateBy { get; set; } = "system";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UpdateBy { get; set; } = "system";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
