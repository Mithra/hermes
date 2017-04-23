using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hermes.Entity.Models
{
    [Table("notification_client_receipt")]
    public class NotificationClientReceipt
    {
        [Key, Column(Order = 1)]
        public Guid client_id { get; set; }

        // Foreign Keys
        [Key, Column(Order = 0)]
        public long notification_id { get; set; }

        // Navigation Properties
        [ForeignKey("notification_id")]
        public virtual Notification Notification { get; set; }
    }
}