using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Domain.Entities
{
    public class FeeType
    {
        [Key]
        public short FeeTypeID { get; set; }   // SQL smallint → C# short

        [StringLength(30)]
        public string FeeTypeName { get; set; }   // nvarchar(30)
        public string FeeTypeCode { get; set; }   // nvarchar(30)

        public bool UpdateBalance { get; set; }   // bit (nullable if column allows NULL)
    }
}