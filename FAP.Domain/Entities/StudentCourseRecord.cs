using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Domain.Entities
{
    public class StudentCourseRecord
    {
        public Guid StudentId { get; private set; }
        public string CourseCode { get; private set; } = string.Empty;

        /// <summary>
        /// Điểm hoặc trạng thái qua môn (true = đã qua)
        /// </summary>
        public bool Passed { get; private set; }

        public decimal? Score { get; private set; }

        private StudentCourseRecord() { }

        public StudentCourseRecord(Guid studentId, string courseCode, bool passed, decimal? score = null)
        {
            StudentId = studentId;
            CourseCode = courseCode;
            Passed = passed;
            Score = score;
        }
    }
}

