using Demos.Domain;

namespace Demos.Demos;

public static class LabeledJumps
{
    public static void Run()
    {
        var screening = Screening.Sample();
        PrintSeatMap(screening);

        foreach (int group in new[] { 3, 5, 9 })
        {
            Console.WriteLine($"{group} seats together (before): {Format(FindAdjacentBefore(screening, group))}");
            Console.WriteLine($"{group} seats together (after):  {Format(FindAdjacent(screening, group))}");
        }
    }

    // C# 14: a flag (or goto) to escape the nested loops
    static Seat? FindAdjacentBefore(Screening screening, int count)
    {
        Seat? result = null;
        bool found = false;
        for (int row = 0; row < screening.Rows && !found; row++)
        {
            for (int start = 0; start + count <= screening.SeatsPerRow; start++)
            {
                bool allFree = true;
                for (int i = 0; i < count; i++)
                {
                    if (screening.Seats[row, start + i].IsTaken)
                    {
                        allFree = false;
                        break;
                    }
                }
                if (!allFree)
                    continue;

                result = screening.Seats[row, start];
                found = true;
                break;
            }
        }
        return result;
    }

    // C# 15: labeled continue and break
    static Seat? FindAdjacent(Screening screening, int count)
    {
        Seat? result = null;

        search: for (int row = 0; row < screening.Rows; row++)
        {
            candidate: for (int start = 0; start + count <= screening.SeatsPerRow; start++)
            {
                for (int i = 0; i < count; i++)
                {
                    if (screening.Seats[row, start + i].IsTaken)
                        continue candidate;     // try the next starting seat
                }

                result = screening.Seats[row, start];
                break search;                   // done — leave all loops
            }
        }

        return result;
    }

    static string Format(Seat? seat) => seat is null ? "none" : $"starting at {seat}";

    static void PrintSeatMap(Screening screening)
    {
        Console.WriteLine("                [ SCREEN ]");
        Console.WriteLine("      " + string.Concat(Enumerable.Range(1, screening.SeatsPerRow).Select(n => $"{n,3}")));
        for (int row = 0; row < screening.Rows; row++)
        {
            Console.Write($"  {(char)('A' + row)}   ");
            for (int s = 0; s < screening.SeatsPerRow; s++)
                Console.Write(screening.Seats[row, s].IsTaken ? "  X" : "  .");
            Console.WriteLine();
        }
    }
}
