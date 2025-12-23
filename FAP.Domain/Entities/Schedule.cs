using System;
using System.Collections.Generic;

namespace FAP.Common.Domain.Entities
{

    public class ScheduleRepeat
    {
        public string RepeatID { get; set; }
        public int ScheduleID { get; set; }
    }

    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int? CourseID { get; set; }
        public Course Course { get; set; }
        public short? SessionNo { get; set; }
        public string Lecturer { get; set; }
        public string RoomNo { get; set; }
        public short AreaID { get; set; }
        public DateTime Date { get; set; }
        public byte Slot { get; set; }
        public string Note { get; set; }
        public string Booker { get; set; }
        public DateTime? RecordTime { get; set; }
        public int Type { get; set; } = 0;
        public int Coaching { get; set; } = 0;
    }
}