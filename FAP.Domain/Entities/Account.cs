using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Domain.Entities
{
    public class Account
    {
        [Key]
        [StringLength(10)]
        public string RollNumber { set; get; }
        public decimal Balance { set; get; }


    }
}