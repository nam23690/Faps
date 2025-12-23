using FAP.Common.Domain.Entities;
using System.Linq;

namespace FAP.Common.Domain.Services
{
    public class PrerequisiteChecker
    {
        public bool SatisfyPrerequisites(
            Course course,
            List<StudentCourseRecord> studentRecords,
            Func<string, List<string>> getEquivalents,
            Func<string, List<string>> getReplacements)
        {
            var passed = studentRecords
                .Where(r => r.Passed)
                .Select(r => r.CourseCode)
                .ToList();

            foreach (var prereq in course.Prerequisites)
            {
                // 1. Đã học rồi
                if (passed.Contains(prereq))
                    continue;

                // 2. Môn tương đương
                var eqs = getEquivalents(prereq);
                if (eqs.Any(x => passed.Contains(x)))
                    continue;

                // 3. Môn thay thế
                var reps = getReplacements(prereq);
                if (reps.Any(x => passed.Contains(x)))
                    continue;

                return false;
            }

            return true;
        }
    }
}
