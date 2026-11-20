using lab2.Models;

namespace lab2.Solvers
{
    public interface ISolver
    {
        string Name { get; }
        SolverResult Solve(PuzzleState start);
    }
}
