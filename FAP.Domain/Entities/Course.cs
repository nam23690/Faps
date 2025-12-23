using System;
using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Domain.Entities
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseDetail { get; set; }
        public short TermID { get; set; }
        public string ClassName { get; set; }
        public short SubjectID { get; set; }
        public short? SyllabusID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte NumberOfSlots { get; set; }
        public bool IsBis { get; set; }
        public bool IsTemp { get; set; }
        public string SlotTypeCode { get; set; }
        public bool IsOpen { get; set; }
        public bool? IsInternation { get; set; }
        public bool IsHeadStart { get; set; }
        public bool IsView { get; set; }
        public List<string> Prerequisites { get; internal set; }
    }


}