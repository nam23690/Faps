using System.Collections.Generic;

namespace FAP.Common.Domain.Entities
{
    public class FeedbackQuestion
    {
        public short QuestionID { get; set; }
        public string Content { get; set; }
        public string Noi_dung { get; set; }

        public ICollection<FeedbackAnswer> FeedbackAnswers { get; set; }
        public ICollection<FeedbackResultDetail> FeedbackResultDetails { get; set; }
    }

}