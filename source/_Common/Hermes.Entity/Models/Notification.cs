using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hermes.Entity.Models
{
    [Table("notification")]
    public class Notification
    {
        [Key]
        public long notification_id { get; set; }

        public DateTime notification_time { get; set; }

        [Required]
        [StringLength(128)]
        public string notification_code { get; set; }

        [Required]
        [StringLength(256)]
        public string notification_message { get; set; }

        // Foreign Keys
        public long channel_id { get; set; }

        // Navigation Properties
        public virtual Channel Channel { get; set; }

        // Inverse FK
        [ForeignKey("notification_id")]
        public virtual ICollection<NotificationTag> NotificationTags { get; set; }
    }
}