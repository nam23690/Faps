using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Domain.Entities
{
    public class Subject
    {
        public short SubjectId { get; set; }

        public string SubjectCode { get; set; }

        public string OldSubjectCode { get; set; }

        public string ShortName { get; set; }

        public string SubjectName { get; set; } = null!;

        public string SubjectGroup { get; set; }

        public string SubjectV { get; set; }

        public bool TakeAttendance { get; set; }

        public string ReplacedBy { get; set; }

        public bool IsGraded { get; set; }

        public bool KeepCredits { get; set; }

        public short? NewestSubjectId { get; set; }
        public int? NumberOfStudent { get; set; }

        public decimal? Fee { get; set; }

        public bool IsBeforeOjt { get; set; }

        public bool IsRequired { get; set; }
        public bool IsTeaching { get; set; }
        public bool IsLetterGraded { get; set; }

        public decimal? FeeIs { get; set; }
    }
}
