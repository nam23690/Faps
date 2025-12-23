namespace FAP.Common.Domain.Entities
{
    public class FeedbackResult
    {
        public int FeedbackID { get; set; }
        public string RollNumber { get; set; }
        public float GPA { get; set; }
        public string Comment { get; set; }

        public Feedback Feedback { get; set; }
    }

}