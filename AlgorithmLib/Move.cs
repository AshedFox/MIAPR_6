namespace AlgorithmLib;

public class Move
{
    public (string name, double val) NewValue { get; set; } = ("", 0);
    public List<(string name, double val)> OldValues { get; set; } = new();
    public List<Move> OldMoves { get; set; } = new();
}