using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Text;

namespace FAP.Common.Domain.Entities
{
    public class Attendance
    {
        public int ScheduleID { get; set; }
        public string RollNumber { get; set; }      // varchar(10)
        public bool? Status { get; set; }           // bit
        public string Comment { get; set; }         // nvarchar(250)
        public string Taker { get; set; }           // varchar(20)
        public DateTime? RecordTime { get; set; }   // smalldatetime
    }

}