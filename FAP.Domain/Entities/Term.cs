using FAP.Common.Domain.Base;
using FAP.Common.Domain.Events;
using System;
using FAP.Common.Domain.Entities; // For Campus

namespace FAP.Common.Domain.Entities
{
    public class Term : BaseEntity
    {
        // Removed TermID, using BaseEntity.Id (int) instead. 
        // If DB has short TermID, we need to handle mapping in Configuration.
        
        public short CampusID { get; private set; }
        public string SemesterName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsClosed { get; private set; }
        
        // Navigation property
        public virtual Campus Campus { get; private set; }

        // EF Core needs a parameterless constructor
        private Term() { }

        // Factory method
        public static Term Create(short campusId, string semesterName, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(semesterName))
                throw new ArgumentException("Semester name cannot be empty.", nameof(semesterName));

            if (endDate <= startDate)
                throw new InvalidOperationException("End date must be after start date.");

            var term = new Term
            {
                CampusID = campusId,
                SemesterName = semesterName,
                StartDate = startDate,
                EndDate = endDate,
                IsClosed = false
            };

            term.AddDomainEvent(new TermCreatedEvent(term));
            term.AddDomainEvent(new TermCreatedEventEmail(term));

            return term;
        }

        public void UpdateInfo(short campusId, string semesterName, DateTime startDate, DateTime endDate, bool isClosed)
        {
            if (string.IsNullOrWhiteSpace(semesterName))
                throw new ArgumentException("Semester name cannot be empty.", nameof(semesterName));

            if (endDate <= startDate)
                throw new InvalidOperationException("End date must be after start date.");

            CampusID = campusId;
            SemesterName = semesterName;
            StartDate = startDate;
            EndDate = endDate;
            IsClosed = isClosed;
        }
        
        public void Close()
        {
            IsClosed = true;
        }
    }
}