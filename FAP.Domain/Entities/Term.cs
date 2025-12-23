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

    }
}