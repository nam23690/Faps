using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FAP.Common.Domain.Entities;

namespace FAP.Common.Domain.Entities
{
    [Table("Blocks")]
    public class Block
    {
        [Key]
        public short BlockID { get; set; }

        [Required]
        [StringLength(255)] // Tùy chỉnh theo độ dài nvarchar trong DB
        public string BlockName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [ForeignKey(nameof(Term))]
        public short TermID { get; set; }

        public virtual Term Term { get; set; }
    }
}