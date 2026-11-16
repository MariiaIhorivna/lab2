using lab2.Models;

namespace lab2.Heuristics
{
    public class LinearConflictHeuristic : IHeuristic
    {
        public string Name => "Manhattan + Linear Conflict";

        public int Calculate(PuzzleState state)
        {
            int manhattan = new ManhattanHeuristic().Calculate(state);
            int conflicts = 0;

            // Рядки
            for (int row = 0; row < 3; row++)
            {
                for (int i = 0; i < 3; i++)
                {
                    int idx1 = row * 3 + i;
                    int tile1 = state.Board[idx1];
                    if (tile1 == 0) continue;
                    int goalRow1 = (tile1 - 1) / 3;
                    if (goalRow1 != row) continue;

                    for (int j = i + 1; j < 3; j++)
                    {
                        int idx2 = row * 3 + j;
                        int tile2 = state.Board[idx2];
                        if (tile2 == 0) continue;
                        int goalRow2 = (tile2 - 1) / 3;
                        if (goalRow2 != row) continue;

                        int goalCol1 = (tile1 - 1) % 3;
                        int goalCol2 = (tile2 - 1) % 3;
                        if (goalCol1 > goalCol2) conflicts += 2;
                    }
                }
            }

            // Стовпці (аналогічно)
            for (int col = 0; col < 3; col++)
            {
                for (int i = 0; i < 3; i++)
                {
                    int idx1 = i * 3 + col;
                    int tile1 = state.Board[idx1];
                    if (tile1 == 0) continue;
                    int goalCol1 = (tile1 - 1) % 3;
                    if (goalCol1 != col) continue;

                    for (int j = i + 1; j < 3; j++)
                    {
                        int idx2 = j * 3 + col;
                        int tile2 = state.Board[idx2];
                        if (tile2 == 0) continue;
                        int goalCol2 = (tile2 - 1) % 3;
                        if (goalCol2 != col) continue;

                        int goalRow1 = (tile1 - 1) / 3;
                        int goalRow2 = (tile2 - 1) / 3;
                        if (goalRow1 > goalRow2) conflicts += 2;
                    }
                }
            }

            return manhattan + conflicts;
        }
    }
}
