using lab2.Experiments;
using lab2.Heuristics;
using lab2.Models;

namespace lab2.Solvers
{
    public class AStarSolver : SearchAlgorithm
    {
        private readonly IHeuristic _heuristic;

        public AStarSolver(IHeuristic heuristic) => _heuristic = heuristic;

        public override string Name => $"A* ({_heuristic.Name})";

        public override SolverResult Solve(PuzzleState start)
        {
            ResetStats();

            try
            {
                if (start.IsGoal())
                    return CreateResult(SearchStatus.Solved, 0, new List<PuzzleState> { start });

                var open = new PriorityQueue<SearchNode, int>();
                var gScore = new Dictionary<PuzzleState, int>();
                var closed = new HashSet<PuzzleState>();

                int h0 = _heuristic.Calculate(start);
                var root = new SearchNode(start, null, 0, h0);
                open.Enqueue(root, root.F);
                gScore[start] = 0;
                GeneratedStates = 1;

                while (open.Count > 0)
                {
                    var current = open.Dequeue();

                    if (closed.Contains(current.State))
                        continue;

                    if (LimitsExceeded(out var status))
                        return CreateResult(status);

                    closed.Add(current.State);
                    Iterations++;

                    if (current.State.IsGoal())
                        return CreateResult(SearchStatus.Solved, current.G, ReconstructPath(current));

                    foreach (var neighbor in current.State.GetNeighbors())
                    {
                        if (closed.Contains(neighbor))
                            continue;

                        int tentativeG = current.G + 1;

                        if (!gScore.TryGetValue(neighbor, out int oldG) || tentativeG < oldG)
                        {
                            GeneratedStates++;
                            int h = _heuristic.Calculate(neighbor);
                            var node = new SearchNode(neighbor, current, tentativeG, h);
                            open.Enqueue(node, node.F);
                            gScore[neighbor] = tentativeG;

                            // Правильний підрахунок пам’яті (без подвійного рахунку)
                            TrackMemory(open.Count + closed.Count);
                        }
                    }
                }

                return CreateResult(SearchStatus.NoSolution);
            }
            catch (OutOfMemoryException)
            {
                return CreateResult(SearchStatus.OutOfMemory);
            }
        }

        private static List<PuzzleState> ReconstructPath(SearchNode node)
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
