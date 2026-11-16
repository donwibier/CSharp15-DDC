using System;

namespace LegacyLib;

// netstandard2.0 has no default-interface-member runtime support.
// Up to C# 14 the static members below were error CS8701 on this target.
// C# 15 allows NON-VIRTUAL static members here (instance defaults and static abstract/virtual still need runtime support).
public interface IPricing
{
    decimal PriceFor(DateTime startsAt);

    public static decimal VatRate => 0.21m;

    public static decimal WithVat(decimal amount) => Math.Round(amount * (1 + VatRate), 2);

    public static IPricing Default { get; } = new StandardPricing();
}

internal sealed class StandardPricing : IPricing
{
    public decimal PriceFor(DateTime startsAt) =>
        IPricing.WithVat(startsAt.Hour < 17 ? 8.00m : 11.00m);
}
