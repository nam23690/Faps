using System.Collections.Generic;
using System;

namespace FAP.Common.Domain.Entities
{
    public class Feedback
    {
        public int FeedbackID { get; set; }
        public int CourseID { get; set; }
        public string Lecturer { get; set; }
        public DateTime OpenDay { get; set; }
        public string Opener { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string ClosingUser { get; set; }
        public string Comment { get; set; }

        public ICollection<FeedbackResult> FeedbackResults { get; set; }
        public ICollection<FeedbackResultDetail> FeedbackResultDetails { get; set; }
    }

}