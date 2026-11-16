using Demos.Domain;

namespace Demos.Demos;

// A collection type whose constructor requires an argument.
public class SeatSelection(string customer) : List<Seat>
{
    public string Customer { get; } = customer;
}

public static class CollectionArguments
{
    public static void Run()
    {
        var screening = Screening.Sample();

        // Before (C# 14): back to the old collection-initializer syntax as soon as you need a constructor argument
        var promoBefore = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "DDC2026", "EARLYBIRD" };

        // C# 15: with(...) as the first element passes arguments to the constructor
        HashSet<string> promoCodes = [with(StringComparer.OrdinalIgnoreCase), "DDC2026", "ddc2026", "EARLYBIRD"];
        Console.WriteLine($"Promo codes: {promoCodes.Count} -> {string.Join(", ", promoCodes)}");
        Console.WriteLine($"'EarlyBird' valid? {promoCodes.Contains("EarlyBird")}");

        // Capacity
        Seat[] allSeats = [.. screening.Seats.Cast<Seat>()];
        List<Seat> free = [with(capacity: allSeats.Length), .. allSeats.Where(s => !s.IsTaken)];
        Console.WriteLine($"Free seats: {free.Count} (capacity {free.Capacity})");

        // Sorted, with a custom comparer
        SortedSet<string> titles = [with(StringComparer.CurrentCultureIgnoreCase), "dune", "Alien", "back to the Future", "Casablanca"];
        Console.WriteLine($"Titles: {string.Join(" | ", titles)}");

        // A required constructor argument
        SeatSelection selection = [with("Don"), screening.Seats[5, 4], screening.Seats[5, 5]];
        Console.WriteLine($"{selection.Customer} picked {string.Join(" & ", selection)}");

        // Interfaces: IList<T>/ICollection<T> accept with() or with(capacity)
        IList<int> rowNumbers = [with(capacity: 10), 1, 2, 3];
        Console.WriteLine($"IList<int>: {string.Join(",", rowNumbers)}");

        _ = promoBefore;
    }
}
