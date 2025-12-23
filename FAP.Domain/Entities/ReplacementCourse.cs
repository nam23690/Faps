namespace FAP.Common.Domain.Entities
{
    public class ReplacementCourse
    {
        public string OriginalCourse { get; private set; } = string.Empty;
        public string Replacement { get; private set; } = string.Empty;

        private ReplacementCourse() { }

        public ReplacementCourse(string originalCourse, string replacement)
        {
            OriginalCourse = originalCourse;
            Replacement = replacement;
        }
    }
}
