using lab2.Models;

namespace lab2.Heuristics
{
    public class ManhattanHeuristic : IHeuristic
    {
        public string Name => "Manhattan";

        public int Calculate(PuzzleState state)
        {
            int distance = 0;
            for (int i = 0; i < 9; i++)
            {
                int tile = state.Board[i];
                if (tile == 0) continue;

                int goalIndex = tile - 1;
                int curRow = i / 3, curCol = i % 3;
                int goalRow = goalIndex / 3, goalCol = goalIndex % 3;
                distance += Math.Abs(curRow - goalRow) + Math.Abs(curCol - goalCol);
            }
            return distance;
        }
    }
}
