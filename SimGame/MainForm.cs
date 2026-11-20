using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimGame
{
    /// <summary>
    /// Головне вікно гри Sim: людина (червоні) проти комп'ютера (сині).
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly AlphaBetaAI _ai = new AlphaBetaAI(Difficulty.Medium);
        private SimGameState _game;
        private bool _computerThinking;
        private int _humanWins;
        private int _computerWins;

        public MainForm()
        {
            InitializeComponent();
            boardView.EdgeClicked += BoardView_EdgeClicked;
            cmbDifficulty.SelectedIndex = 1;   // «Середній» за замовчуванням
            StartNewGame();
        }

        // ------------------------------------------------------------------
        // Обробники подій
        // ------------------------------------------------------------------

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void cmbDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDifficulty.SelectedIndex >= 0)
                _ai.Difficulty = (Difficulty)cmbDifficulty.SelectedIndex;
        }

        /// <summary>Хід людини: клік по вільному відрізку.</summary>
        private void BoardView_EdgeClicked(object sender, int edge)
        {
            if (_game.IsOver || _computerThinking || _game.CurrentPlayer != SimGameState.HumanColor) return;
            if (!_game.MakeMove(edge)) return;

            AddHistory(edge, SimGameState.HumanColor);
            CountResultIfOver();
            RefreshView();

            if (!_game.IsOver) MakeComputerMove();
        }

        // ------------------------------------------------------------------
        // Логіка гри
        // ------------------------------------------------------------------

        private void StartNewGame()
        {
            _game = new SimGameState(rbHumanFirst.Checked);
            _computerThinking = false;
            boardView.LastComputerEdge = -1;
            boardView.Game = _game;
            lstHistory.Items.Clear();
            lblAiInfo.Text = "Комп'ютер ще не ходив.";
            RefreshView();

            if (_game.CurrentPlayer == SimGameState.ComputerColor) MakeComputerMove();
        }

        /// <summary>
        /// Хід комп'ютера. Пошук виконується у фоновому потоці, щоб вікно не «зависало».
        /// </summary>
        private async void MakeComputerMove()
        {
            SimGameState game = _game;
            _computerThinking = true;
            RefreshView();

            SimBoard snapshot = game.Board.Clone();
            SearchInfo info = null;
            int move = await Task.Run(() => _ai.ChooseMove(snapshot, SimGameState.ComputerColor, out info));

            // Невелика пауза, щоб гравець встиг побачити свій хід перед відповіддю
            int pause = 300 - (int)info.Milliseconds;
            if (pause > 0) await Task.Delay(pause);

            if (game != _game) return;          // поки комп'ютер думав, почали нову гру

            _game.MakeMove(move);
            boardView.LastComputerEdge = move;
            AddHistory(move, SimGameState.ComputerColor);
            ShowSearchInfo(info);
            CountResultIfOver();

            _computerThinking = false;
            RefreshView();
        }

        private void RefreshView()
        {
            boardView.InputEnabled = !_game.IsOver && !_computerThinking &&
                                     _game.CurrentPlayer == SimGameState.HumanColor;

            if (_game.IsOver)
            {
                string triangle = _game.LosingTriangle == null ? "" :
                    $"{_game.LosingTriangle[0] + 1}–{_game.LosingTriangle[1] + 1}–{_game.LosingTriangle[2] + 1}";
                if (_game.Winner == SimGameState.HumanColor)
                {
                    lblStatus.Text = $"Ви виграли! Комп'ютер утворив синій трикутник {triangle}.";
                    lblStatus.ForeColor = Color.FromArgb(0, 128, 0);
                }
                else
                {
                    lblStatus.Text = $"Ви програли: утворено червоний трикутник {triangle}.";
                    lblStatus.ForeColor = Color.FromArgb(200, 30, 30);
                }
            }
            else if (_computerThinking)
            {
                lblStatus.Text = "Комп'ютер думає…";
                lblStatus.ForeColor = Color.FromArgb(31, 94, 204);
            }
            else
            {
                lblStatus.Text = "Ваш хід: клацніть по вільному відрізку.";
                lblStatus.ForeColor = Color.FromArgb(45, 52, 64);
            }

            boardView.Invalidate();
        }

        /// <summary>Викликається після кожного ходу: якщо партія завершилась – оновлює рахунок.</summary>
        private void CountResultIfOver()
        {
            if (!_game.IsOver) return;
            if (_game.Winner == SimGameState.HumanColor) _humanWins++;
            else _computerWins++;
            lblScore.Text = $"Рахунок: гравець {_humanWins} : {_computerWins} комп'ютер";
        }

        private void AddHistory(int edge, PlayerColor color)
        {
            string who = color == SimGameState.HumanColor ? "Гравець" : "Комп'ютер";
            lstHistory.Items.Add($"{_game.History.Count,2}. {who}: {SimBoard.EdgeName(edge)}");
            lstHistory.TopIndex = Math.Max(0, lstHistory.Items.Count - 1);
        }

        private void ShowSearchInfo(SearchInfo info)
        {
            string level = cmbDifficulty.Text;
            string score;
            if (info.Score > AlphaBetaAI.WinScore / 2)
                score = $"{info.Score} (виграш гарантовано)";
            else if (info.Score < -AlphaBetaAI.WinScore / 2)
                score = $"{info.Score} (програш, якщо суперник не помилиться)";
            else
                score = $"{info.Score} (евристична оцінка)";

            lblAiInfo.Text =
                $"Хід: {SimBoard.EdgeName(info.Move)}   рівень: {level}\n" +
                $"Глибина пошуку: {info.Depth}\n" +
                $"Переглянуто позицій: {info.Nodes:N0}\n" +
                $"Відсікань α-β: {info.Cutoffs:N0}\n" +
                $"Оцінка: {score}\n" +
                $"Час: {info.Milliseconds:0.0} мс";
        }
    }
}
