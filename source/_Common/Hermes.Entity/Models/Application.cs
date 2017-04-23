using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hermes.Entity.Models
{
    [Table("application")]
    public class Application
    {
        [Key]
        public int application_id { get; set; }

        [Required]
        [StringLength(128)]
        [Index("IX_application_name", IsUnique = true)]
        public string application_name { get; set; }
    }
}