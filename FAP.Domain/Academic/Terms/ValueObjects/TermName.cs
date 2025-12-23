namespace FAP.Common.Domain.Academic.Terms.ValueObjects;

public sealed class TermName
{
    public string Value { get; }

    public TermName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Term name is required");

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
