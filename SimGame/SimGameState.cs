using System.Collections.Generic;

namespace SimGame
{
    /// <summary>
    /// Партія гри Sim: поле, чий хід, історія ходів, визначення переможця.
    /// Інтерфейс (форма) працює тільки через цей клас.
    /// </summary>
    public sealed class SimGameState
    {
        public const PlayerColor HumanColor = PlayerColor.Red;
        public const PlayerColor ComputerColor = PlayerColor.Blue;

        public SimBoard Board { get; private set; } = new SimBoard();
        public PlayerColor CurrentPlayer { get; private set; }
        public bool IsOver { get; private set; }

        /// <summary>Хто програв (утворив трикутник свого кольору).</summary>
        public PlayerColor Loser { get; private set; }

        /// <summary>Вершини трикутника, яким завершилась гра.</summary>
        public int[] LosingTriangle { get; private set; }

        public List<int> History { get; } = new List<int>();

        public PlayerColor Winner => IsOver ? SimBoard.Opponent(Loser) : PlayerColor.None;

        public SimGameState(bool humanFirst)
        {
            CurrentPlayer = humanFirst ? HumanColor : ComputerColor;
        }

        /// <summary>Зробити хід поточним гравцем. Повертає false, якщо хід неможливий.</summary>
        public bool MakeMove(int edge)
        {
            if (IsOver || edge < 0 || edge >= SimBoard.EdgeCount || !Board.IsFree(edge)) return false;

            bool losing = Board.CompletesTriangle(edge, CurrentPlayer);
            Board.Apply(edge, CurrentPlayer);
            History.Add(edge);

            if (losing)
            {
                IsOver = true;
                Loser = CurrentPlayer;
                LosingTriangle = Board.FindTriangle(CurrentPlayer);
            }
            else
            {
                CurrentPlayer = SimBoard.Opponent(CurrentPlayer);
            }
            return true;
        }
    }
}
