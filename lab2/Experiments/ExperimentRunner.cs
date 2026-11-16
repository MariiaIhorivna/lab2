using lab2.Heuristics;
using lab2.Models;
using lab2.Solvers;

namespace lab2.Experiments
{
    public class ExperimentRunner
    {
        private readonly TimeSpan _timeLimit;

        public ExperimentRunner(TimeSpan? timeLimit = null)
        {
            _timeLimit = timeLimit ?? TimeSpan.FromMinutes(5);
        }

        public void Run(int count = 25)
        {
            Console.WriteLine($"=== Серія з {count} експериментів ===\n");

            var solvers = new List<ISolver>
            {
                new BfsSolver(),
                new AStarSolver(new ManhattanHeuristic()),
                new AStarSolver(new LinearConflictHeuristic())
            };

            // Генеруємо однакові початкові стани
            var starts = new List<PuzzleState>();
            for (int i = 0; i < count; i++)
                starts.Add(PuzzleState.GenerateRandomSolvable());

            foreach (var solver in solvers)
            {
                Console.WriteLine($"Алгоритм: {solver.Name}");
                Console.WriteLine(new string('=', 60));

                var results = new List<SolverResult>();

                for (int i = 0; i < starts.Count; i++)
                {
                    Console.Write($"  Експеримент {i + 1,2}/{count} ... ");

                    var result = solver.Solve(starts[i], _timeLimit);
                    results.Add(result);

                    if (result.Success)
                        Console.WriteLine($"OK  (кроків: {result.Steps,3}, згенеровано: {result.GeneratedStates,7})");
                    else
                        Console.WriteLine($"FAIL / TIMEOUT ({result.Message})");

                    // ===== ВИВІД ШЛЯХУ ТІЛЬКИ ДЛЯ ПЕРШОГО ЕКСПЕРИМЕНТУ =====
                    if (result.Success)
                    {
                        Console.WriteLine($"\n  --- Шлях для {i}-го експерименту ---");
                        PrintPath(result);
                        Console.WriteLine("  ---------------------------------\n");
                    }
                }

                PrintStatistics(results);
                Console.WriteLine();
            }

            // Таблиця початкових станів
            Console.WriteLine("=== Початкові стани (для звіту) ===");
            for (int i = 0; i < starts.Count; i++)
            {
                Console.WriteLine($"#{i + 1,2}: [{string.Join(",", starts[i].Board)}]");
            }
        }

        /// <summary>
        /// Виводить послідовність станів (як рухається порожня клітинка)
        /// </summary>
        private static void PrintPath(SolverResult result)
        {
            if (!result.Success || result.Path == null || result.Path.Count == 0)
            {
                Console.WriteLine("  Шлях відсутній (розв’язок не знайдено)");
                return;
            }

            Console.WriteLine($"  Довжина шляху: {result.Path.Count - 1} ходів\n");

            for (int step = 0; step < result.Path.Count; step++)
            {
                Console.WriteLine($"  Крок {step}:");
                // Додаємо відступ, щоб було гарно
                var lines = result.Path[step].ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                    Console.WriteLine("    " + line);
                Console.WriteLine();
            }
        }

        private static void PrintStatistics(List<SolverResult> results)
        {
            var successful = results.Where(r => r.Success).ToList();

            Console.WriteLine();
            Console.WriteLine($"  Успішних розв’язків:          {successful.Count} / {results.Count}");

            if (successful.Any())
            {
                Console.WriteLine($"  Середня к-сть кроків:           {successful.Average(r => r.Steps):F2}");
                Console.WriteLine($"  Середня к-сть ітерацій:         {successful.Average(r => r.Iterations):F2}");
                Console.WriteLine($"  Середня к-сть згенерованих:     {successful.Average(r => r.GeneratedStates):F2}");
                Console.WriteLine($"  Середня макс. станів у пам’яті:  {successful.Average(r => r.MaxStatesInMemory):F2}");
            }
            else
            {
                Console.WriteLine("  (немає успішних розв’язків)");
            }
        }
    }
}