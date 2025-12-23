using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAP.Common.Domain.Entities
{
    public class FeePlan
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string RollNumber { get; set; }

        [StringLength(500)]
        public string Term { get; set; }

        [StringLength(1500)]
        public string FullName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Fee { get; set; }

        [StringLength(200)]
        public string Note { get; set; }

        public DateTime? EntryDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public int? TermNo { get; set; }

        public bool? IsCheck { get; set; }

        public DateTime? DueDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RealFee { get; set; }

        [StringLength(50)]
        public string FeeNId { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(20)]
        public string CreateBy { get; set; }

        [StringLength(20)]
        public string SubjectCode { get; set; }

        [StringLength(50)]
        public string BatchNo { get; set; }

        [StringLength(50)]
        public string ReceiptNo { get; set; }

        public DateTime? ReceiptDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        public bool? IsRefun { get; set; }

        [StringLength(50)]
        public string Type_FeeN { get; set; }

        [StringLength(50)]
        public string EstimateTime { get; set; }

    }
}