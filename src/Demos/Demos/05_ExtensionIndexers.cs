using Demos.Domain;

namespace Demos.Demos;

// A row of seats. Note: no indexer, no Count — it's someone else's type.
public sealed class SeatRow(Seat[] seats)
{
    public Seat[] Seats { get; } = seats;
}

public static class ScreeningExtensions
{
    // C# 14 gave us extension blocks with properties. C# 15 adds indexers.
    extension(Screening screening)
    {
        public Seat this[char row, int number] => screening.Seats[row - 'A', number - 1];

        public Seat this[string code] => screening[code[0], int.Parse(code[1..])];

        public SeatRow this[char row] =>
            new([.. Enumerable.Range(1, screening.SeatsPerRow).Select(n => screening[row, n])]);
    }

    // Count + int indexer + Slice => ^1, ranges and list patterns light up too.
    extension(SeatRow row)
    {
        public int Count => row.Seats.Length;
        public Seat this[int index] => row.Seats[index];
        public SeatRow Slice(int start, int length) => new(row.Seats.AsSpan(start, length).ToArray());
    }
}

public static class ExtensionIndexers
{
    public static void Run()
    {
        var screening = Screening.Sample();

        // Before: screening.Seats['F' - 'A', 12 - 1]  — who wants to read that?
        Console.WriteLine($"screening['F', 10] = {screening['F', 10]}");
        Console.WriteLine($"screening[\"A3\"] taken? {screening["A3"].IsTaken}");

        SeatRow rowC = screening['C'];
        Console.WriteLine($"Row C last seat:  {rowC[^1]}");
        Console.WriteLine($"Row C seats 3-5:  {string.Join(", ", rowC[2..5].Seats.Select(s => s.ToString()))}");

        bool aisleFree = rowC is [{ IsTaken: false }, .., { IsTaken: false }];
        Console.WriteLine($"Both aisle seats in row C free? {aisleFree}");
    }
}
