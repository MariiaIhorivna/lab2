using lab2.Models;
using lab2.Solvers;
using System.Diagnostics;

namespace lab2.Experiments
{
    public enum SearchStatus
    {
        Solved,
        TimeLimit,
        MemoryLimit,
        NoSolution,
        OutOfMemory
    }

    public abstract class SearchAlgorithm : ISolver
    {
        public abstract string Name { get; }

        protected static class Limits
        {
            public static readonly TimeSpan TimeLimit = TimeSpan.FromMinutes(5);
            public static readonly long MemoryLimitBytes = 1L * 1024 * 1024 * 1024; // 1 ГБ
            public const int CheckEvery = 1024;
        }

        protected int Iterations;
        protected int GeneratedStates;
        protected int MaxStatesInMemory;
        protected readonly Stopwatch Stopwatch = new();

        protected void ResetStats()
        {
            Iterations = 0;
            GeneratedStates = 0;
            MaxStatesInMemory = 0;
            Stopwatch.Restart();
        }

        /// <summary>
        /// Перевірка лімітів. Викликати періодично (раз на CheckEvery ітерацій).
        /// </summary>
        protected bool LimitsExceeded(out SearchStatus status)
        {
            status = SearchStatus.Solved;

            if (Iterations % Limits.CheckEvery != 0)
                return false;

            // Час
            if (Stopwatch.Elapsed > Limits.TimeLimit)
            {
                status = SearchStatus.TimeLimit;
                return true;
            }

            if (GC.GetTotalMemory(false) > Limits.MemoryLimitBytes)
            {
                status = SearchStatus.MemoryLimit;
                return true;
            }

            return false;
        }

        protected void TrackMemory(int nodesInMemory)
        {
            if (nodesInMemory > MaxStatesInMemory)
                MaxStatesInMemory = nodesInMemory;
        }

        protected SolverResult CreateResult(SearchStatus status, int steps = 0, List<PuzzleState>? path = null)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            return new SolverResult
            {
                Success = status == SearchStatus.Solved,
                Steps = steps,
                Iterations = Iterations,
                GeneratedStates = GeneratedStates,
                MaxStatesInMemory = MaxStatesInMemory,
                Time = Stopwatch.Elapsed,
                Path = path ?? new List<PuzzleState>(),
                Message = status switch
                {
                    SearchStatus.TimeLimit => $"Час вичерпано ({Limits.TimeLimit.TotalMinutes} хв)",
                    SearchStatus.MemoryLimit => $"Перевищено ліміт пам’яті ({Limits.MemoryLimitBytes / 1024 / 1024 / 1024} ГБ)",
                    SearchStatus.OutOfMemory => "OutOfMemoryException",
                    SearchStatus.NoSolution => "Розв’язок не знайдено",
                    _ => null
                }
            };
#pragma warning restore CS8601 
        }

        public abstract SolverResult Solve(PuzzleState start);
    }
}
