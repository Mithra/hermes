using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hermes.Entity.Models
{
    [Table("notification_tag")]
    public class NotificationTag
    {
        [Key]
        public long notification_tag_id { get; set; }

        [Required]
        [StringLength(128)]
        public string notification_tag_key { get; set; }

        [Required]
        [StringLength(128)]
        public string notification_tag_value { get; set; }

        // Foreign Keys
        public long notification_id { get; set; }

        // Navigation Properties
        public Notification Notification { get; set; }
    }
}