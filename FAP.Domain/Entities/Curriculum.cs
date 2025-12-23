using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Domain.Entities
{
    public class Curriculum
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string CurriculumCode { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        [StringLength(250)]
        public string Subject1 { get; set; }
        [StringLength(250)]
        public string Condition1 { get; set; }
        [StringLength(250)]
        public string Subject2 { get; set; }
        [StringLength(250)]
        public string Condition2 { get; set; }
        public int? TotalCredit { get; set; }
    }

}