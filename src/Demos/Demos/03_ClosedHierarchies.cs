namespace Demos.Demos.Closed;

// C# 15: 'closed' = only this assembly may derive directly from ShowStatus.
// A closed class is implicitly abstract.
public closed record class ShowStatus
{
    // Unlike a union, a closed hierarchy can share state and behavior via the base class.
    public bool CanBook => this is OnSale { SeatsLeft: > 0 };
}

public record class Scheduled(DateTime SalesOpenAt) : ShowStatus;
public record class OnSale(int SeatsLeft) : ShowStatus;
public record class SoldOut : ShowStatus;
public record class Cancelled(string Reason) : ShowStatus;

// In another assembly:
// public record class Postponed : ShowStatus;   // error: 'ShowStatus' is a closed class

public static class ClosedHierarchies
{
    public static void Run()
    {
        ShowStatus[] statuses =
        [
            new Scheduled(new DateTime(2026, 11, 1)),
            new OnSale(42),
            new OnSale(0),
            new SoldOut(),
            new Cancelled("Projector on fire"),
        ];

        foreach (var status in statuses)
            Console.WriteLine($"{Describe(status),-45} can book: {status.CanBook}");

        ShowStatus? unknown = null;
        Console.WriteLine(DescribeNullable(unknown));
    }

    // No discard needed — and no UnreachableException. Every direct descendant is handled.
    static string Describe(ShowStatus status) => status switch
    {
        Scheduled(var opens) => $"Tickets on sale from {opens:d MMM}",
        OnSale(0) => "On sale, but no seats left (!)",
        OnSale(var left) => $"On sale – {left} seats left",
        SoldOut => "Sold out",
        Cancelled(var reason) => $"Cancelled: {reason}",
    };

    // Nullable? Then you need a null arm — the compiler tells you.
    static string DescribeNullable(ShowStatus? status) => status switch
    {
        null => "No status yet",
        var s => Describe(s),
    };
}
