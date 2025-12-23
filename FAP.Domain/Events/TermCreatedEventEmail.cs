using FAP.Common.Domain.Academic.Terms;
using FAP.Common.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Domain.Events
{
    public class TermCreatedEventEmail:IDomainEvent
    {
        public Academic.Terms.Term term { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public TermCreatedEventEmail(Academic.Terms.Term _term)
        {
            term = _term;
        }
    }
}
