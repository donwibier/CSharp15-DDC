namespace Demos.Domain;

public record class Movie(string Title, int Minutes);

public record class Seat(char Row, int Number)
{
    public bool IsTaken { get; set; }
    public override string ToString() => $"{Row}{Number}";
}

public sealed class Screening(Movie movie, DateTime startsAt, int rows, int seatsPerRow)
{
    public Movie Movie { get; } = movie;
    public DateTime StartsAt { get; } = startsAt;

    // Plain 2D grid — no friendly indexer. Extension indexers fix that in demo 04.
    public Seat[,] Seats { get; } = CreateGrid(rows, seatsPerRow);

    public int Rows => Seats.GetLength(0);
    public int SeatsPerRow => Seats.GetLength(1);

    static Seat[,] CreateGrid(int rows, int seatsPerRow)
    {
        var grid = new Seat[rows, seatsPerRow];
        for (int r = 0; r < rows; r++)
            for (int s = 0; s < seatsPerRow; s++)
                grid[r, s] = new Seat((char)('A' + r), s + 1);
        return grid;
    }

    public static Screening Sample()
    {
        var screening = new Screening(new Movie("Back to the Future", 116), new DateTime(2026, 11, 24, 20, 15, 0), 6, 10);
        // Take some seats so the demos have something to work with.
        foreach (var (row, number) in new[] { ('A', 3), ('A', 4), ('B', 1), ('B', 2), ('B', 3), ('B', 4), ('B', 5), ('B', 6), ('B', 7), ('B', 8),
                                               ('C', 2), ('C', 6), ('D', 5), ('D', 6), ('D', 7), ('E', 1) })
            screening.Seats[row - 'A', number - 1].IsTaken = true;
        return screening;
    }
}
