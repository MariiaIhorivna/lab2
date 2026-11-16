using lab2.Models;

namespace lab2.Heuristics
{
    public interface IHeuristic
    {
        int Calculate(PuzzleState state);
        string Name { get; }
    }
}