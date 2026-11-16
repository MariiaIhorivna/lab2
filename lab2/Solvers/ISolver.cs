using lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.Solvers
{
    public interface ISolver
    {
        string Name { get; }
        SolverResult Solve(PuzzleState start, TimeSpan timeLimit);
    }
}
