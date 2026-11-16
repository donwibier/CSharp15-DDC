using System.Text.Json;
using System.Text.Json.Serialization;
using Demos.Domain;

namespace Demos.Demos.After;

// The case types are ordinary records. They don't share a base class.
public record class Booked(string TicketCode, Seat Seat, decimal Price);
public record class SoldOut(Movie Movie);
public record class SeatTaken(Seat Seat, Seat? Alternative);
public record class PaymentDeclined(string Reason);

// C# 15: one line, and the compiler knows the full set of outcomes.
// (The attribute is only needed for JSON deserialization — see the end of Run.)
[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union BookingResult(Booked, SoldOut, SeatTaken, PaymentDeclined);

// Case types don't have to be related at all — even int and string.
public union SeatRef(int, string);

// A generic union.
public record class None;
public record class Some<T>(T Value);
public union Option<T>(None, Some<T>);

// A union can have a body with members (no instance fields though).
public record struct Percentage(decimal Value);
public record struct FixedAmount(decimal Value);

public union Discount(Percentage, FixedAmount)
{
    public decimal ApplyTo(decimal price) => this switch
    {
        Percentage p => Math.Round(price * (1 - p.Value / 100m), 2),
        FixedAmount f => Math.Max(0, price - f.Value),
        _ => price, // default(Discount) holds no value
    };
}

public static class BookingService
{
    public static BookingResult Book(Screening screening, SeatRef seat, string card, Discount discount = default)
    {
        Seat target = seat switch
        {
            int number => FindByNumber(screening, number),
            string code => screening.Seats[code[0] - 'A', int.Parse(code[1..]) - 1],
            // no discard needed: int and string are all there is
        };

        if (target.IsTaken)
            return new SeatTaken(target, FindAlternative(screening, target) switch
            {
                Some<Seat> s => s.Value,
                None => null,
            });

        if (card.StartsWith("0000"))
            return new PaymentDeclined("Insufficient funds");   // implicit conversion to BookingResult

        target.IsTaken = true;
        return new Booked($"DDC-{target}", target, discount.ApplyTo(12.50m));
    }

    static Option<Seat> FindAlternative(Screening screening, Seat taken)
    {
        int row = taken.Row - 'A';
        for (int s = 0; s < screening.SeatsPerRow; s++)
            if (!screening.Seats[row, s].IsTaken)
                return new Some<Seat>(screening.Seats[row, s]);
        return new None();
    }

    static Seat FindByNumber(Screening screening, int number) =>
        screening.Seats[(number - 1) / screening.SeatsPerRow, (number - 1) % screening.SeatsPerRow];
}

public static class UnionsAfter
{
    public static void Run()
    {
        var screening = Screening.Sample();

        Console.WriteLine(Describe(BookingService.Book(screening, "F10", "4111")));
        Console.WriteLine(Describe(BookingService.Book(screening, "A3", "4111")));
        Console.WriteLine(Describe(BookingService.Book(screening, "B5", "4111")));
        Console.WriteLine(Describe(BookingService.Book(screening, 25, "0000")));
        Console.WriteLine(Describe(BookingService.Book(screening, 42, "4111", new Percentage(20))));
        Console.WriteLine(Describe(BookingService.Book(screening, "F1", "4111", new FixedAmount(5))));

        // BookingService.Book(screening, 12.5, "4111");   // ← compile error now: double is not a SeatRef case

        BookingResult result = new SoldOut(screening.Movie);
        Console.WriteLine($"Value: {result.Value}");

        // System.Text.Json in .NET 11 understands unions: it writes the active case...
        string json = JsonSerializer.Serialize(result);
        Console.WriteLine($"JSON:  {json}");

        // ...and reads it back. Primitive cases are picked by JSON value kind;
        // object cases are told apart by their shape thanks to the structural classifier.
        var back = JsonSerializer.Deserialize<BookingResult>(json);
        Console.WriteLine($"Back:  {back.Value?.GetType().Name}");

        SeatRef fromJson = JsonSerializer.Deserialize<SeatRef>("\"F12\"");
        Console.WriteLine($"SeatRef from \"F12\": {fromJson.Value?.GetType().Name} {fromJson.Value}");
    }

    // No discard. Add a case to BookingResult and this switch lights up with a warning.
    static string Describe(BookingResult result) => result switch
    {
        Booked b => $"Booked {b.Seat} – ticket {b.TicketCode} (€{b.Price})",
        SoldOut s => $"{s.Movie.Title} is sold out",
        SeatTaken(var seat, { } alternative) => $"Seat {seat} is taken – how about {alternative}?",
        SeatTaken t => $"Seat {t.Seat} is taken and the row is full",
        PaymentDeclined p => $"Payment declined: {p.Reason}",
    };
}
