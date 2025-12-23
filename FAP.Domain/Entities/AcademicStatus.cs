using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Domain.Entities
{
    public class AcademicStatus
    {
        [Key]
        [StringLength(10)]
        public string StatusCode { set; get; }
        [StringLength(50)]
        public string StatusName { set; get; }
        public bool CanJoinToCourse { set; get; }

    }
}