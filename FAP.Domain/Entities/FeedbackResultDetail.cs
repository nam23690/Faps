namespace FAP.Common.Domain.Entities
{
    public class FeedbackResultDetail
    {
        public int FeedbackID { get; set; }
        public string RollNumber { get; set; }
        public short QuestionID { get; set; }
        public byte AnswerCode { get; set; }

        public Feedback Feedback { get; set; }
        public FeedbackQuestion FeedbackQuestion { get; set; }
    }

}