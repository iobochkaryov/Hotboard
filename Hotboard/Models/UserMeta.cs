using System;
using System.ComponentModel.DataAnnotations;

namespace Hotboard.Models
{
    public class UserMeta
    {
        [Key]
        public string UserId { get; set; } = null!;
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}