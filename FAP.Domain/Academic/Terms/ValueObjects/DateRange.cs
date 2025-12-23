namespace FAP.Common.Domain.Academic.Terms.ValueObjects;

public sealed class DateRange
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    // 🔒 EF Core constructor
    private DateRange() { }

    // ✅ Domain constructor
    public DateRange(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new ArgumentException("StartDate must be before EndDate");

        StartDate = start;
        EndDate = end;
    }

    public bool Overlaps(DateRange other)
        => StartDate < other.EndDate && EndDate > other.StartDate;
}
