using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FnbReservationAPI.src.features.Staff
{
    public class Staff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "char(36)")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column(TypeName = "char(36)")]
        public Guid UserId { get; set; }

        [Required]
        [Column(TypeName = "char(36)")]
        public Guid OutletId { get; set; }  

        public string CreateBy { get; set; } = "system";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UpdateBy { get; set; } = "system";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
    }
}
