using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hermes.Entity.Models
{
    [Table("channel")]
    public class Channel
    {
        [Key]
        public long channel_id { get; set; }

        [Required]
        [StringLength(128)]
        [Index("IX_ApplicationChannel", 1, IsUnique = true)]
        public string channel_name { get; set; }

        // Foreign Keys
        [Index("IX_ApplicationChannel", 2, IsUnique = true)]
        public int application_id { get; set; }

        // Navigation Properties
        public virtual Application Application { get; set; }
    }
}