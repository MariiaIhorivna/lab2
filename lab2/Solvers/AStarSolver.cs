using lab2.Heuristics;
using lab2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.Solvers
{
    public class AStarSolver : ISolver
    {
        private readonly IHeuristic _heuristic;

        public AStarSolver(IHeuristic heuristic)
        {
            _heuristic = heuristic;
        }

        public string Name => $"A* ({_heuristic.Name})";

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

            var open = new PriorityQueue<SearchNode, int>();
            var gScore = new Dictionary<PuzzleState, int>();
            var closed = new HashSet<PuzzleState>();

            int h0 = _heuristic.Calculate(start);
            var root = new SearchNode(start, null, 0, h0);
            open.Enqueue(root, root.F);
            gScore[start] = 0;

            int generated = 1;
            int maxMemory = 1;

            while (open.Count > 0)
            {
                if (sw.Elapsed > timeLimit)
                {
                    result.Message = "Час вичерпано";
                    result.GeneratedStates = generated;
                    result.MaxStatesInMemory = maxMemory;
                    result.Time = sw.Elapsed;
                    return result;
                }

                var current = open.Dequeue();
                result.Iterations++;

                if (closed.Contains(current.State)) continue;
                closed.Add(current.State);

                if (current.State.IsGoal())
                {
                    result.Success = true;
                    result.Steps = current.G;
                    result.GeneratedStates = generated;
                    result.MaxStatesInMemory = maxMemory;
                    result.Time = sw.Elapsed;
                    result.Path = ReconstructPath(current);
                    return result;
                }

                foreach (var neighbor in current.State.GetNeighbors())
                {
                    if (closed.Contains(neighbor)) continue;

                    int tentativeG = current.G + 1;

                    if (!gScore.TryGetValue(neighbor, out int oldG) || tentativeG < oldG)
                    {
                        generated++;
                        int h = _heuristic.Calculate(neighbor);
                        var node = new SearchNode(neighbor, current, tentativeG, h);
                        open.Enqueue(node, node.F);
                        gScore[neighbor] = tentativeG;
                        maxMemory = Math.Max(maxMemory, open.Count + closed.Count + gScore.Count);
                    }
                }
            }

            result.Message = "Розв’язок не знайдено";
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
