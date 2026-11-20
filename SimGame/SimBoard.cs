using System;

namespace SimGame
{
    /// <summary>Колір відрізка: не зафарбований, червоний (гравець) або синій (комп'ютер).</summary>
    public enum PlayerColor : byte
    {
        None = 0,
        Red = 1,
        Blue = 2
    }

    /// <summary>
    /// Ігрове поле гри Sim: 6 вершин шестикутника, усі 15 відрізків між ними.
    /// Стан зберігається двома бітовими масками (біт i = відрізок i):
    /// RedMask – червоні відрізки, BlueMask – сині.
    /// Хто першим утворить трикутник (3 вершини) зі своїх відрізків – програє.
    /// </summary>
    public sealed class SimBoard
    {
        public const int VertexCount = 6;
        public const int EdgeCount = 15;
        public const int AllEdgesMask = (1 << EdgeCount) - 1;

        /// <summary>Вершини, які з'єднує відрізок i.</summary>
        public static readonly int[] EdgeFrom = new int[EdgeCount];
        public static readonly int[] EdgeTo = new int[EdgeCount];

        /// <summary>Номер відрізка між вершинами a і b.</summary>
        private static readonly int[,] EdgeIndex = new int[VertexCount, VertexCount];

        /// <summary>
        /// Для кожного відрізка – 4 трикутники, у які він входить.
        /// Кожен трикутник записаний маскою з ДВОХ інших його сторін.
        /// </summary>
        private static readonly int[][] TrianglePairs = new int[EdgeCount][];

        /// <summary>Усі 20 трикутників (маски з трьох сторін).</summary>
        public static readonly int[] TriangleMasks = new int[20];

        static SimBoard()
        {
            int e = 0;
            for (int a = 0; a < VertexCount; a++)
                for (int b = a + 1; b < VertexCount; b++)
                {
                    EdgeFrom[e] = a;
                    EdgeTo[e] = b;
                    EdgeIndex[a, b] = EdgeIndex[b, a] = e;
                    e++;
                }

            for (int i = 0; i < EdgeCount; i++) TrianglePairs[i] = new int[4];
            var filled = new int[EdgeCount];
            int t = 0;
            for (int a = 0; a < VertexCount; a++)
                for (int b = a + 1; b < VertexCount; b++)
                    for (int c = b + 1; c < VertexCount; c++)
                    {
                        int ab = EdgeIndex[a, b], ac = EdgeIndex[a, c], bc = EdgeIndex[b, c];
                        TriangleMasks[t++] = (1 << ab) | (1 << ac) | (1 << bc);
                        TrianglePairs[ab][filled[ab]++] = (1 << ac) | (1 << bc);
                        TrianglePairs[ac][filled[ac]++] = (1 << ab) | (1 << bc);
                        TrianglePairs[bc][filled[bc]++] = (1 << ab) | (1 << ac);
                    }
        }

        public int RedMask { get; private set; }
        public int BlueMask { get; private set; }

        public int FreeMask => AllEdgesMask & ~(RedMask | BlueMask);
        public int MovesMade => BitCount(RedMask | BlueMask);

        public static int GetEdge(int a, int b) => EdgeIndex[a, b];

        public int MaskOf(PlayerColor color) =>
            color == PlayerColor.Red ? RedMask : color == PlayerColor.Blue ? BlueMask : 0;

        public static PlayerColor Opponent(PlayerColor color) =>
            color == PlayerColor.Red ? PlayerColor.Blue : PlayerColor.Red;

        public bool IsFree(int edge) => (FreeMask & (1 << edge)) != 0;

        public PlayerColor GetColor(int edge)
        {
            if ((RedMask & (1 << edge)) != 0) return PlayerColor.Red;
            if ((BlueMask & (1 << edge)) != 0) return PlayerColor.Blue;
            return PlayerColor.None;
        }

        /// <summary>Чи утворить гравець із маскою mask трикутник, якщо зафарбує відрізок edge.</summary>
        public static bool CompletesTriangle(int mask, int edge)
        {
            int[] pairs = TrianglePairs[edge];
            for (int i = 0; i < pairs.Length; i++)
                if ((mask & pairs[i]) == pairs[i]) return true;
            return false;
        }

        public bool CompletesTriangle(int edge, PlayerColor color) => CompletesTriangle(MaskOf(color), edge);

        /// <summary>Зафарбувати відрізок.</summary>
        public void Apply(int edge, PlayerColor color)
        {
            if (!IsFree(edge)) throw new InvalidOperationException("Відрізок уже зафарбований.");
            if (color == PlayerColor.Red) RedMask |= 1 << edge;
            else BlueMask |= 1 << edge;
        }

        /// <summary>Повертає три вершини трикутника кольору color (або null, якщо його немає).</summary>
        public int[] FindTriangle(PlayerColor color)
        {
            int mask = MaskOf(color);
            for (int a = 0; a < VertexCount; a++)
                for (int b = a + 1; b < VertexCount; b++)
                    for (int c = b + 1; c < VertexCount; c++)
                    {
                        int tri = (1 << EdgeIndex[a, b]) | (1 << EdgeIndex[a, c]) | (1 << EdgeIndex[b, c]);
                        if ((mask & tri) == tri) return new[] { a, b, c };
                    }
            return null;
        }

        public SimBoard Clone()
        {
            return new SimBoard { RedMask = RedMask, BlueMask = BlueMask };
        }

        public static int BitCount(int x)
        {
            int count = 0;
            while (x != 0) { x &= x - 1; count++; }
            return count;
        }

        public static string EdgeName(int edge) => $"{EdgeFrom[edge] + 1}–{EdgeTo[edge] + 1}";
    }
}
