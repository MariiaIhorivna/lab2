using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimGame
{
    public enum Difficulty
    {
        Easy,     // глибина 1: оцінює лише власний хід
        Medium,   // глибина 3: свій хід, відповідь гравця, ще один свій хід
        Hard      // повний перебір до кінця гри (з таблицею транспозицій)
    }

    /// <summary>Статистика останнього пошуку – показується у вікні гри.</summary>
    public sealed class SearchInfo
    {
        public int Move;
        public int Depth;
        public int Score;
        public long Nodes;
        public long Cutoffs;
        public double Milliseconds;
    }

    /// <summary>
    /// Комп'ютерний опонент: мінімакс з альфа-бета відсіканнями.
    /// MAX – комп'ютер, MIN – людина. Позиція = дві бітові маски (свої і чужі відрізки).
    ///
    /// Кінцеві стани: хто зафарбував відрізок і утворив трикутник свого кольору – програв.
    /// Оцінка виграшу/програшу = ±(WinScore − номер ходу), тому комп'ютер обирає
    /// найшвидший виграш і, якщо програш неминучий, – найпізніший програш.
    /// Якщо глибина вичерпана, використовується евристична оцінка Evaluate().
    /// </summary>
    public sealed class AlphaBetaAI
    {
        public const int WinScore = 10000;
        private const int Infinity = int.MaxValue / 2;

        private readonly Random _random = new Random();
        private long _nodes;
        private long _cutoffs;

        // Таблиця транспозицій (лише для складного рівня): одна й та сама позиція
        // може виникнути з різним порядком ходів – вдруге її не рахуємо.
        private Dictionary<int, TableEntry> _table;

        private struct TableEntry
        {
            public int Depth;
            public int Value;
            public byte Flag; // 0 – точне значення, 1 – нижня межа, 2 – верхня межа
        }

        public Difficulty Difficulty { get; set; }

        public AlphaBetaAI(Difficulty difficulty)
        {
            Difficulty = difficulty;
        }

        public static int DepthFor(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy: return 1;
                case Difficulty.Medium: return 3;
                default: return SimBoard.EdgeCount; // до кінця гри
            }
        }

        /// <summary>
        /// ALPHA-BETA-SEARCH: вибирає найкращий хід для кольору me.
        /// </summary>
        public int ChooseMove(SimBoard board, PlayerColor me, out SearchInfo info)
        {
            var timer = Stopwatch.StartNew();
            _nodes = 1;                 // корінь дерева пошуку
            _cutoffs = 0;
            _table = Difficulty == Difficulty.Hard ? new Dictionary<int, TableEntry>() : null;

            int my = board.MaskOf(me);
            int opp = board.MaskOf(SimBoard.Opponent(me));
            int free = SimBoard.AllEdgesMask & ~(my | opp);
            int depth = Math.Min(DepthFor(Difficulty), SimBoard.BitCount(free));

            // Корінь: перемішуємо ходи, щоб при однакових оцінках гра не була однаковою,
            // а потім упорядковуємо їх евристикою (рівні за оцінкою залишаються у випадковому порядку)
            List<int> moves = OrderedMoves(my, opp, true, shuffle: true);

            int alpha = -Infinity, beta = Infinity;
            int bestMove = moves[0], bestValue = -Infinity;
            foreach (int move in moves)
            {
                int value = SimBoard.CompletesTriangle(my, move)
                    ? -(WinScore - 1)                                         // хід одразу програє
                    : MinValue(my | (1 << move), opp, depth - 1, alpha, beta, 1);

                if (value > bestValue)
                {
                    bestValue = value;
                    bestMove = move;
                }
                if (bestValue > alpha) alpha = bestValue;
            }

            // Складний рівень: якщо при ідеальній грі суперника позиція програшна
            // (так буває, коли комп'ютер ходить першим), об'єктивно кращого ходу немає.
            // Тоді комп'ютер грає змішану стратегію: з імовірністю 1/2 залишає хід,
            // що найдовше відтягує програш (знайдений альфа-бета пошуком), інакше ставить
            // «пастку» – хід, після якого у людини найменша частка правильних відповідей.
            if (Difficulty == Difficulty.Hard && bestValue < 0 && _random.Next(2) == 0)
                bestMove = ChooseTrapMove(my, opp, moves, bestMove, depth);

            timer.Stop();
            info = new SearchInfo
            {
                Move = bestMove,
                Depth = depth,
                Score = bestValue,
                Nodes = _nodes,
                Cutoffs = _cutoffs,
                Milliseconds = timer.Elapsed.TotalMilliseconds
            };
            _table = null;
            return bestMove;
        }

        /// <summary>
        /// Для кожного безпечного ходу рахує, яка частка відповідей суперника зберігає йому виграш.
        /// Кожна відповідь перевіряється альфа-бета пошуком з вузьким вікном (−1; 1): нам потрібен
        /// лише знак результату (виграш/програш), тому відсікань дуже багато, а таблиця
        /// транспозицій спільна для всіх перевірок.
        /// </summary>
        private int ChooseTrapMove(int my, int opp, List<int> moves, int fallback, int depth)
        {
            int bestMove = fallback;
            double bestShare = double.MaxValue;
            foreach (int move in moves)
            {
                if (SimBoard.CompletesTriangle(my, move)) continue;
                int afterMy = my | (1 << move);
                int free = SimBoard.AllEdgesMask & ~(afterMy | opp);

                int replies = 0, goodReplies = 0;
                for (int r = 0; r < SimBoard.EdgeCount; r++)
                {
                    if ((free & (1 << r)) == 0 || SimBoard.CompletesTriangle(opp, r)) continue;
                    replies++;
                    int value = MaxValue(afterMy, opp | (1 << r), depth - 2, -1, 1, 2);
                    if (value < 0) goodReplies++;      // після цієї відповіді комп'ютер усе одно програє
                }
                if (replies == 0) return move;          // суперник не має безпечних ходів – він програє

                double share = (double)goodReplies / replies;
                if (share < bestShare)
                {
                    bestShare = share;
                    bestMove = move;
                }
            }
            return bestMove;
        }

        /// <summary>MAX-VALUE: хід комп'ютера (my – його відрізки).</summary>
        private int MaxValue(int my, int opp, int depth, int alpha, int beta, int ply)
        {
            _nodes++;
            int free = SimBoard.AllEdgesMask & ~(my | opp);
            if (depth == 0 || free == 0) return Evaluate(my, opp);

            int key = 0, alphaOriginal = alpha, betaOriginal = beta;
            if (_table != null && Probe(my, opp, depth, ref alpha, ref beta, out key, out int stored)) return stored;

            int value = -Infinity;
            foreach (int move in OrderedMoves(my, opp, true, false))
            {
                int v = SimBoard.CompletesTriangle(my, move)
                    ? -(WinScore - (ply + 1))                                 // комп'ютер утворив свій трикутник
                    : MinValue(my | (1 << move), opp, depth - 1, alpha, beta, ply + 1);
                if (v > value) value = v;
                if (value >= beta) { _cutoffs++; break; }                   // β-відсікання
                if (value > alpha) alpha = value;
            }

            if (_table != null) Store(key, depth, value, alphaOriginal, betaOriginal);
            return value;
        }

        /// <summary>MIN-VALUE: хід людини (opp – її відрізки).</summary>
        private int MinValue(int my, int opp, int depth, int alpha, int beta, int ply)
        {
            _nodes++;
            int free = SimBoard.AllEdgesMask & ~(my | opp);
            if (depth == 0 || free == 0) return Evaluate(my, opp);

            int key = 0, alphaOriginal = alpha, betaOriginal = beta;
            if (_table != null && Probe(my, opp, depth, ref alpha, ref beta, out key, out int stored)) return stored;

            int value = Infinity;
            foreach (int move in OrderedMoves(my, opp, false, false))
            {
                int v = SimBoard.CompletesTriangle(opp, move)
                    ? WinScore - (ply + 1)                                    // людина утворила свій трикутник
                    : MaxValue(my, opp | (1 << move), depth - 1, alpha, beta, ply + 1);
                if (v < value) value = v;
                if (value <= alpha) { _cutoffs++; break; }                  // α-відсікання
                if (value < beta) beta = value;
            }

            if (_table != null) Store(key, depth, value, alphaOriginal, betaOriginal);
            return value;
        }

        /// <summary>
        /// Евристична оцінка позиції з точки зору комп'ютера.
        /// Гра програється тоді, коли не залишилось «безпечних» ходів, тобто
        /// ходів, які не замикають власний трикутник. Тому:
        ///   + 10 за кожен безпечний хід комп'ютера, − 10 за кожен безпечний хід людини;
        ///   − 3 за кожен «потенційно небезпечний» трикутник комп'ютера
        ///     (одна сторона його кольору, дві вільні), + 3 – такий самий трикутник людини.
        /// </summary>
        public static int Evaluate(int my, int opp)
        {
            int free = SimBoard.AllEdgesMask & ~(my | opp);
            int safeMy = 0, safeOpp = 0;
            for (int e = 0; e < SimBoard.EdgeCount; e++)
            {
                if ((free & (1 << e)) == 0) continue;
                if (!SimBoard.CompletesTriangle(my, e)) safeMy++;
                if (!SimBoard.CompletesTriangle(opp, e)) safeOpp++;
            }

            int riskMy = 0, riskOpp = 0;
            foreach (int tri in SimBoard.TriangleMasks)
            {
                int mine = SimBoard.BitCount(tri & my);
                int theirs = SimBoard.BitCount(tri & opp);
                if (mine == 1 && theirs == 0) riskMy++;
                if (theirs == 1 && mine == 0) riskOpp++;
            }
            return 10 * (safeMy - safeOpp) - 3 * (riskMy - riskOpp);
        }

        /// <summary>
        /// Упорядкування ходів: спочатку «безпечні» (не замикають свій трикутник),
        /// серед них – кращі за швидкою оцінкою. Гарний порядок = більше відсікань.
        /// </summary>
        private List<int> OrderedMoves(int my, int opp, bool computerToMove, bool shuffle)
        {
            int free = SimBoard.AllEdgesMask & ~(my | opp);
            var moves = new List<int>(SimBoard.EdgeCount);
            for (int e = 0; e < SimBoard.EdgeCount; e++)
                if ((free & (1 << e)) != 0) moves.Add(e);

            if (shuffle)
            {
                for (int i = moves.Count - 1; i > 0; i--)
                {
                    int j = _random.Next(i + 1);
                    int tmp = moves[i]; moves[i] = moves[j]; moves[j] = tmp;
                }
            }

            int mover = computerToMove ? my : opp;
            int other = computerToMove ? opp : my;
            var keys = new int[SimBoard.EdgeCount];
            foreach (int e in moves) keys[e] = QuickScore(mover, other, e);

            // Стабільне сортування вставками за спаданням оцінки (ходів не більше 15)
            for (int i = 1; i < moves.Count; i++)
            {
                int m = moves[i], k = keys[m], j = i - 1;
                while (j >= 0 && keys[moves[j]] < k) { moves[j + 1] = moves[j]; j--; }
                moves[j + 1] = m;
            }
            return moves;
        }

        /// <summary>Швидка оцінка ходу edge для гравця з маскою mover (лише для порядку перебору).</summary>
        private static int QuickScore(int mover, int other, int edge)
        {
            if (SimBoard.CompletesTriangle(mover, edge)) return -1000;   // програшний хід – у самий кінець
            int score = -2 * SimBoard.BitCount(Neighbours(mover, edge)); // менше своїх сусідніх відрізків – менший ризик
            if (SimBoard.CompletesTriangle(other, edge)) score -= 5;      // цей хід суперник зробити не може – краще лишити «про запас»
            return score;
        }

        /// <summary>Свої відрізки, що мають спільну вершину з edge (чим їх більше, тим ризикованіше).</summary>
        private static int Neighbours(int mask, int edge)
        {
            int a = SimBoard.EdgeFrom[edge], b = SimBoard.EdgeTo[edge], result = 0;
            for (int e = 0; e < SimBoard.EdgeCount; e++)
            {
                if (e == edge || (mask & (1 << e)) == 0) continue;
                if (SimBoard.EdgeFrom[e] == a || SimBoard.EdgeTo[e] == a ||
                    SimBoard.EdgeFrom[e] == b || SimBoard.EdgeTo[e] == b) result |= 1 << e;
            }
            return result;
        }

        // ---------------- Таблиця транспозицій ----------------

        private bool Probe(int my, int opp, int depth, ref int alpha, ref int beta, out int key, out int value)
        {
            key = my | (opp << SimBoard.EdgeCount);
            value = 0;
            if (!_table.TryGetValue(key, out TableEntry entry) || entry.Depth < depth) return false;

            if (entry.Flag == 0) { value = entry.Value; return true; }
            if (entry.Flag == 1 && entry.Value > alpha) alpha = entry.Value;
            if (entry.Flag == 2 && entry.Value < beta) beta = entry.Value;
            if (alpha >= beta) { value = entry.Value; _cutoffs++; return true; }
            return false;
        }

        private void Store(int key, int depth, int value, int alphaOriginal, int betaOriginal)
        {
            byte flag = value <= alphaOriginal ? (byte)2 : value >= betaOriginal ? (byte)1 : (byte)0;
            _table[key] = new TableEntry { Depth = depth, Value = value, Flag = flag };
        }
    }
}
