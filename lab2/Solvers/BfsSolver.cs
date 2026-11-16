using lab2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.Solvers
{
    public class BfsSolver : ISolver
    {
        public string Name => "BFS (АНП) — AS IS (без перевірки циклів)";

        public SolverResult Solve(PuzzleState start, TimeSpan timeLimit)
        {
            var sw = Stopwatch.StartNew();
            var result = new SolverResult();

            if (start.IsGoal())
            {
                result.Success = true;
                result.Path.Add(start);
                return result;
            }

            var queue = new Queue<SearchNode>();
            var root = new SearchNode(start);
            queue.Enqueue(root);

            int generated = 1;
            int maxMemory = 1;

            while (queue.Count > 0)
            {
                // Обмеження за часом
                if (sw.Elapsed > timeLimit)
                {
                    result.Message = $"Час вичерпано ({timeLimit.Minutes} хв)";
                    result.GeneratedStates = generated;
                    result.MaxStatesInMemory = maxMemory;
                    result.Time = sw.Elapsed;
                    return result;
                }

                var current = queue.Dequeue();
                result.Iterations++;

                // Генеруємо сусідів БЕЗ будь-якої перевірки на повтор
                foreach (var neighbor in current.State.GetNeighbors())
                {
                    generated++;
                    var node = new SearchNode(neighbor, current, current.G + 1);

                    if (neighbor.IsGoal())
                    {
                        result.Success = true;
                        result.Steps = node.G;
                        result.GeneratedStates = generated;
                        result.MaxStatesInMemory = maxMemory;
                        result.Time = sw.Elapsed;
                        result.Path = ReconstructPath(node);
                        return result;
                    }

                    queue.Enqueue(node);
                    maxMemory = Math.Max(maxMemory, queue.Count);
                }
            }

            result.Message = "Черга спорожніла (майже неможливо)";
            result.GeneratedStates = generated;
            result.MaxStatesInMemory = maxMemory;
            result.Time = sw.Elapsed;
            return result;
        }

        private List<PuzzleState> ReconstructPath(SearchNode node)
        {
            var path = new List<PuzzleState>();
            while (node != null)
            {
                path.Add(node.State);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}
