using System.Diagnostics;
using Demos.Domain;

namespace Demos.Demos.Before;

// The classic C# 14 way: an abstract base class + derived records.
// Works, but the compiler has no idea the list of results is complete.
public abstract record class BookingResult;
public sealed record class Booked(string TicketCode, Seat Seat, decimal Price) : BookingResult;
public sealed record class SoldOut(Movie Movie) : BookingResult;
public sealed record class SeatTaken(Seat Seat, Seat? Alternative) : BookingResult;
public sealed record class PaymentDeclined(string Reason) : BookingResult;

// Anyone (in any assembly!) can add a new result...
// public sealed record class Refunded(string TicketCode) : BookingResult;

public static class BookingService
{
    // "Seat as a number OR as a code" — without unions you pick object, overloads, or a library like OneOf<int, string>.
    public static BookingResult Book(Screening screening, object seat, string card)
    {
        Seat target = seat switch
        {
            int number => FindByNumber(screening, number),
            string code => screening.Seats[code[0] - 'A', int.Parse(code[1..]) - 1],
            _ => throw new ArgumentException("Seat must be an int or a string", nameof(seat)), // runtime error, not compile time
        };

        if (target.IsTaken)
            return new SeatTaken(target, null);
        if (card.StartsWith("0000"))
            return new PaymentDeclined("Insufficient funds");

        target.IsTaken = true;
        return new Booked($"DDC-{target}", target, 12.50m);
    }

    static Seat FindByNumber(Screening screening, int number) =>
        screening.Seats[(number - 1) / screening.SeatsPerRow, (number - 1) % screening.SeatsPerRow];
}

public static class UnionsBefore
{
    public static void Run()
    {
        var screening = Screening.Sample();

        Console.WriteLine(Describe(BookingService.Book(screening, "F10", "4111")));
        Console.WriteLine(Describe(BookingService.Book(screening, "A3", "4111")));
        Console.WriteLine(Describe(BookingService.Book(screening, 25, "0000")));

        try
        {
            BookingService.Book(screening, 12.5, "4111"); // compiles fine... 
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Runtime surprise: {ex.Message}");
        }
    }

    static string Describe(BookingResult result) => result switch
    {
        Booked b => $"Booked {b.Seat} – ticket {b.TicketCode} (€{b.Price})",
        SoldOut s => $"{s.Movie.Title} is sold out",
        SeatTaken t => $"Seat {t.Seat} is already taken",
        PaymentDeclined p => $"Payment declined: {p.Reason}",
        _ => throw new UnreachableException(), // required: the compiler can't prove we're complete
    };
}
