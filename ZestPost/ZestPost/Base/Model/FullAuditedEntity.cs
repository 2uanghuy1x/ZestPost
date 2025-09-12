using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZestPost.Base.Model
{
    public abstract class FullAuditedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng Id
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo

        public DateTime? UpdatedAt { get; set; } // Thời gian sửa, nullable

        public DateTime? DeletedAt { get; set; } // Thời gian xóa mềm, nullable

        [Required]
        public bool IsDelete { get; set; } = false; // Cờ xóa mềm, mặc định false
    }
}
