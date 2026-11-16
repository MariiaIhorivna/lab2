using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab2.Models
{
    public class PuzzleState : IEquatable<PuzzleState>
    {
        public int[] Board { get; }          // 9 елементів, 0 = порожня
        public int BlankIndex { get; }

        public static readonly int[] Goal = { 1, 2, 3, 4, 5, 6, 7, 8, 0 };

        public PuzzleState(int[] board)
        {
            if (board == null || board.Length != 9)
                throw new ArgumentException("Board must have exactly 9 elements");

            Board = (int[])board.Clone();
            BlankIndex = Array.IndexOf(Board, 0);
        }

        public bool IsGoal() => Board.SequenceEqual(Goal);

        public IEnumerable<PuzzleState> GetNeighbors()
        {
            int row = BlankIndex / 3;
            int col = BlankIndex % 3;

            // Вгору, вниз, вліво, вправо
            var moves = new (int dr, int dc)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

            foreach (var (dr, dc) in moves)
            {
                int newRow = row + dr;
                int newCol = col + dc;

                if (newRow >= 0 && newRow < 3 && newCol >= 0 && newCol < 3)
                {
                    int newIndex = newRow * 3 + newCol;
                    int[] newBoard = (int[])Board.Clone();
                    newBoard[BlankIndex] = newBoard[newIndex];
                    newBoard[newIndex] = 0;
                    yield return new PuzzleState(newBoard);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 9; i++)
            {
                sb.Append(Board[i] == 0 ? "  " : $"{Board[i]} ");
                if ((i + 1) % 3 == 0) sb.AppendLine();
            }
            return sb.ToString();
        }

        public bool Equals(PuzzleState other)
        {
            if (other is null) return false;
            return Board.SequenceEqual(other.Board);
        }

        public override bool Equals(object obj) => Equals(obj as PuzzleState);

        public override int GetHashCode()
        {
            // Простий і швидкий хеш для 9 елементів
            int hash = 17;
            foreach (int x in Board)
                hash = hash * 31 + x;
            return hash;
        }

        // Перевірка розв’язності (парність інверсій)
        public static bool IsSolvable(int[] board)
        {
            int inversions = 0;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == 0) continue;
                for (int j = i + 1; j < 9; j++)
                {
                    if (board[j] != 0 && board[i] > board[j])
                        inversions++;
                }
            }
            return inversions % 2 == 0; // для стандартної цілі
        }

        public static PuzzleState GenerateRandomSolvable()
        {
            var rnd = new Random();
            int[] board;
            do
            {
                board = Enumerable.Range(0, 9).OrderBy(_ => rnd.Next()).ToArray();
            } while (!IsSolvable(board));
            return new PuzzleState(board);
        }
    }
}