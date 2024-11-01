using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Api.Domain.Common
{
    public class SqlBaseEntity
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Active { get; set; } = true;
        public DateTime DateCreate { get; set; } = DateTime.Now;
        public DateTime? DateUpdate { get; set; }
    }
}
