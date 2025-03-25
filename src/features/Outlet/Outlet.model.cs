using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FnbReservationAPI.src.features.Outlet
{
    public class Outlet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "char(36)")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Location { get; set; }

        [Required]
        [MaxLength(10)]
        public required string OperateStartTime { get; set; }

        [Required]  
        [MaxLength(10)]
        public required string OperateEndTime { get; set; }

        [Required]
        public int Capacity { get; set; }

        public string CreateBy { get; set; } = "system";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UpdateBy { get; set; } = "system";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
    }

}
