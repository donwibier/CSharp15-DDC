using LegacyLib;

namespace Demos.Demos;

public static class StaticInterfaceMembers
{
    public static void Run()
    {
        // IPricing lives in a netstandard2.0 library (think: also used by an old .NET Framework app).
        // C# 15 allows non-virtual static members on interfaces there — see LegacyLib/IPricing.cs.
        Console.WriteLine($"€10.00 incl. VAT = €{IPricing.WithVat(10.00m)}");
        Console.WriteLine($"Matinee price: €{IPricing.Default.PriceFor(new DateTime(2026, 11, 24, 14, 0, 0))}");
        Console.WriteLine($"Evening price: €{IPricing.Default.PriceFor(new DateTime(2026, 11, 24, 20, 15, 0))}");
    }
}
