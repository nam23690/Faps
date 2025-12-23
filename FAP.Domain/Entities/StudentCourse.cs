using System;

namespace FAP.Common.Domain.Entities
{
    public class StudentCourse
    {
        public int CourseID { get; set; }
        public string RollNumber { get; set; }
        public bool? TakeAttendance { get; set; }
        public string Note { get; set; }
        public string Login { get; set; }
        public DateTime? RecordTime { get; set; }
        public bool? IsCancelled { get; set; }
        public byte? Characteristic { get; set; }
        public string TypeCode { get; set; }
    }

}