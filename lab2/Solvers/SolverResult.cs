using lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.Solvers
{
    public class SolverResult
    {
        public bool Success { get; set; }
        public int Steps { get; set; }                 // довжина шляху
        public int Iterations { get; set; }            // кількість ітерацій
        public int GeneratedStates { get; set; }
        public int MaxStatesInMemory { get; set; }
        public TimeSpan Time { get; set; }
        public List<PuzzleState> Path { get; set; } = new();
        public string Message { get; set; }
    }
}
