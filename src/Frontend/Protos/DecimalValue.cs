namespace CustomTypes;

public partial class DecimalValue
{
    private const decimal NanoFactor = 1_000_000_000m;

    private DecimalValue(decimal value)
    {
        Units = (long)decimal.Truncate(value);
        Nanos = (int)((value - Units) * NanoFactor);
    }

    public static implicit operator decimal(DecimalValue value) =>
        value.Units + (value.Nanos / NanoFactor);

    public static implicit operator DecimalValue(decimal value) => new(value);

    public static implicit operator DecimalValue(double value) =>
        new(Convert.ToDecimal(value));
}