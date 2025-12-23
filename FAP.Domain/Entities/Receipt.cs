using System.ComponentModel.DataAnnotations;
using System;

namespace FAP.Common.Domain.Entities
{
    public class Receipt
    {
        public string ReceiptNo { get; set; } = null!;
        public short FeeTypeID { get; set; }
        public FeeType FeeType { get; set; }
        public string RollNumber { get; set; } = null!;
        public DateTime ReceiptDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public string Fullname { get; set; }
        public string Login { get; set; }
        public int? TermId { get; set; }
        public int? CourseId { get; set; }
        public bool? IsTemp { get; set; }
        public string Cus_transactionId { get; set; }
        public string PosCode { get; set; }
        public DateTime? EnTryTime { get; set; }
        public bool? IsUpdate { get; set; }
        public string ItemId { get; set; }
        public bool? isFeeN { get; set; }
        public string FeeTypeDNG { get; set; }
        public string InvoiceSerialNumber { get; set; }
        public string CheckSum { get; set; }
        public DateTime? InvoiceDateDNG { get; set; }
    }

}