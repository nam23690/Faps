namespace FAP.Common.Domain.Entities
{
    public class FeedbackAnswer
    {
        public short QuestionID { get; set; }
        public byte AnswerCode { get; set; }
        public string Content { get; set; }
        public string Noi_dung { get; set; }

        public FeedbackQuestion FeedbackQuestion { get; set; }
    }

}