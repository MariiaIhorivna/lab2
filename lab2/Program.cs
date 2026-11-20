using lab2.Experiments;
using lab2.Heuristics;
using lab2.Models;
using lab2.Solvers;
using static System.Runtime.InteropServices.JavaScript.JSType;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== 8-Puzzle Solver ===\n");

// ----- Тестовий початковий стан -----
//var testState = new PuzzleState(new[] { 4, 5, 7, 8, 0, 6, 1, 3, 2 });
// Можна взяти інший, наприклад складніший:
// var testState = new PuzzleState(new[] { 7, 2, 4, 5, 0, 6, 8, 3, 1 });
//if (!PuzzleState.IsSolvable(testState.Board))
//{
//    Console.WriteLine("Таку конфігурацію неможливо розв'язати, спробуйте іншу.");
//} else
//{
//    Console.WriteLine("Тестовий початковий стан:");
//    Console.WriteLine(testState);

//    var timeLimit = TimeSpan.FromMinutes(1);

//    // ========== BFS ==========
//    var bfs = new BfsSolver();
//    var resBfs = bfs.Solve(testState, timeLimit);
//    PrintResult(bfs.Name, resBfs);
//    PrintPath(resBfs);          // ← вивід шляху

//    // ========== A* Манхеттен ==========
//    var astarManhattan = new AStarSolver(new ManhattanHeuristic());
//    var resAManhattan = astarManhattan.Solve(testState, timeLimit);
//    PrintResult(astarManhattan.Name, resAManhattan);
//    PrintPath(resAManhattan);   // ← вивід шляху

//    // ========== A* власна евристика ==========
//    var astarOwn = new AStarSolver(new LinearConflictHeuristic());
//    var resAOwn = astarOwn.Solve(testState, timeLimit);
//    PrintResult(astarOwn.Name, resAOwn);
//    PrintPath(resAOwn);         // ← вивід шляху
//}

// ----- Серія експериментів -----
Console.WriteLine();
var runner = new ExperimentRunner();
runner.Run(count: 20);

// Допоміжний метод для виводу результату
static void PrintResult(string name, SolverResult r)
{
    Console.WriteLine($"--- {name} ---");
    Console.WriteLine($"Успіх: {r.Success}");
    Console.WriteLine($"Кроків: {r.Steps}");
    Console.WriteLine($"Ітерацій: {r.Iterations}");
    Console.WriteLine($"Згенеровано станів: {r.GeneratedStates}");
    Console.WriteLine($"Макс. станів у пам’яті: {r.MaxStatesInMemory}");
    Console.WriteLine($"Час: {r.Time.TotalMilliseconds:F1} мс");
    if (!string.IsNullOrEmpty(r.Message))
        Console.WriteLine($"Повідомлення: {r.Message}");
    Console.WriteLine();
}

// Допоміжний метод для виводу шляху
static void PrintPath(SolverResult result)
{
    if (!result.Success || result.Path == null || result.Path.Count == 0)
    {
        Console.WriteLine("  Шлях відсутній (розв’язок не знайдено)\n");
        return;
    }

    Console.WriteLine($"  === Шлях розв’язку ({result.Path.Count - 1} ходів) ===");

    for (int step = 0; step < result.Path.Count; step++)
    {
        Console.WriteLine($"  Крок {step}:");
        var lines = result.Path[step].ToString()
                         .Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
            Console.WriteLine("    " + line);
        Console.WriteLine();
    }
    Console.WriteLine(new string('-', 40) + "\n");
}