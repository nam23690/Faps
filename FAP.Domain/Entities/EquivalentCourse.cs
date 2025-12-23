namespace FAP.Common.Domain.Entities
{
    public class EquivalentCourse
    {
        public string CourseCode { get; private set; } = string.Empty;
        public string EquivalentCode { get; private set; } = string.Empty;

        private EquivalentCourse() { }

        public EquivalentCourse(string courseCode, string equivalentCode)
        {
            CourseCode = courseCode;
            EquivalentCode = equivalentCode;
        }
    }
}
