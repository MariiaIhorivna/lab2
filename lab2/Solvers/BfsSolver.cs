using lab2.Experiments;
using lab2.Models;

namespace lab2.Solvers
{
    public class BfsSolver : SearchAlgorithm
    {
        public override string Name => "BFS (АНП)";

        public override SolverResult Solve(PuzzleState start)
        {
            ResetStats();

            try
            {
                if (start.IsGoal())
                    return CreateResult(SearchStatus.Solved, 0, new List<PuzzleState> { start });

                var queue = new Queue<SearchNode>();
                queue.Enqueue(new SearchNode(start));
                GeneratedStates = 1;
                TrackMemory(1);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    Iterations++;

                    if (LimitsExceeded(out var status))
                        return CreateResult(status);

                    foreach (var neighbor in current.State.GetNeighbors())
                    {
                        GeneratedStates++;
                        var node = new SearchNode(neighbor, current, current.G + 1);

                        if (neighbor.IsGoal())
                            return CreateResult(SearchStatus.Solved, node.G, ReconstructPath(node));

                        queue.Enqueue(node);
                        TrackMemory(queue.Count);
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
