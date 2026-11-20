using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SimGame
{
    /// <summary>
    /// Елемент керування, що малює поле гри Sim (6 точок і 15 відрізків)
    /// та повідомляє, по якому відрізку клацнув користувач.
    /// </summary>
    public class BoardView : Control
    {
        private static readonly Color HumanColor = Color.FromArgb(214, 39, 40);
        private static readonly Color ComputerColor = Color.FromArgb(31, 94, 204);
        private static readonly Color FreeColor = Color.FromArgb(190, 190, 190);
        private const float HitDistance = 12f;

        private SimGameState _game;
        private int _hoverEdge = -1;

        /// <summary>Подія: користувач клацнув по вільному відрізку (аргумент – номер відрізка).</summary>
        public event EventHandler<int> EdgeClicked;

        public BoardView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = Color.White;
        }

        /// <summary>Партія, яку треба відобразити.</summary>
        public SimGameState Game
        {
            get { return _game; }
            set { _game = value; _hoverEdge = -1; Invalidate(); }
        }

        /// <summary>Останній хід комп'ютера (підсвічується).</summary>
        public int LastComputerEdge { get; set; } = -1;

        /// <summary>Чи може користувач зараз ходити.</summary>
        public bool InputEnabled { get; set; }

        // ------------------------------------------------------------------
        // Геометрія
        // ------------------------------------------------------------------

        /// <summary>Координати 6 вершин правильного шестикутника.</summary>
        private PointF[] GetVertices()
        {
            var points = new PointF[SimBoard.VertexCount];
            float cx = ClientSize.Width / 2f, cy = ClientSize.Height / 2f;
            float radius = Math.Min(ClientSize.Width, ClientSize.Height) * 0.40f;
            for (int i = 0; i < points.Length; i++)
            {
                double angle = Math.PI / 180 * (-90 + 60 * i);
                points[i] = new PointF(cx + radius * (float)Math.Cos(angle), cy + radius * (float)Math.Sin(angle));
            }
            return points;
        }

        /// <summary>Найближчий до точки вільний відрізок (або -1).</summary>
        private int FindEdgeAt(Point p)
        {
            if (_game == null) return -1;
            PointF[] v = GetVertices();
            int best = -1;
            float bestDistance = HitDistance;
            for (int e = 0; e < SimBoard.EdgeCount; e++)
            {
                if (!_game.Board.IsFree(e)) continue;
                float d = DistanceToSegment(p, v[SimBoard.EdgeFrom[e]], v[SimBoard.EdgeTo[e]]);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    best = e;
                }
            }
            return best;
        }

        private static float DistanceToSegment(PointF p, PointF a, PointF b)
        {
            float dx = b.X - a.X, dy = b.Y - a.Y;
            float t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));
            float px = a.X + t * dx - p.X, py = a.Y + t * dy - p.Y;
            return (float)Math.Sqrt(px * px + py * py);
        }

        // ------------------------------------------------------------------
        // Малювання
        // ------------------------------------------------------------------

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);
            if (_game == null) return;

            PointF[] v = GetVertices();
            SimBoard board = _game.Board;

            // Трикутник, яким закінчилась гра, – жовта «підсвітка» під лініями
            if (_game.IsOver && _game.LosingTriangle != null)
            {
                int[] t = _game.LosingTriangle;
                using (var glow = new Pen(Color.FromArgb(255, 200, 0), 18f))
                {
                    glow.StartCap = glow.EndCap = LineCap.Round;
                    g.DrawLine(glow, v[t[0]], v[t[1]]);
                    g.DrawLine(glow, v[t[1]], v[t[2]]);
                    g.DrawLine(glow, v[t[0]], v[t[2]]);
                }
            }

            // Спочатку вільні відрізки, потім зафарбовані (щоб кольорові були зверху)
            using (var freePen = new Pen(FreeColor, 2f) { DashStyle = DashStyle.Dash })
            using (var hoverPen = new Pen(Color.FromArgb(150, HumanColor), 7f))
            {
                for (int edge = 0; edge < SimBoard.EdgeCount; edge++)
                {
                    if (!board.IsFree(edge)) continue;
                    Pen pen = edge == _hoverEdge && InputEnabled ? hoverPen : freePen;
                    g.DrawLine(pen, v[SimBoard.EdgeFrom[edge]], v[SimBoard.EdgeTo[edge]]);
                }
            }

            for (int edge = 0; edge < SimBoard.EdgeCount; edge++)
            {
                PlayerColor color = board.GetColor(edge);
                if (color == PlayerColor.None) continue;
                PointF a = v[SimBoard.EdgeFrom[edge]], b = v[SimBoard.EdgeTo[edge]];

                if (edge == LastComputerEdge)
                {
                    using (var mark = new Pen(Color.FromArgb(120, ComputerColor), 14f))
                    {
                        mark.StartCap = mark.EndCap = LineCap.Round;
                        g.DrawLine(mark, a, b);
                    }
                }
                using (var pen = new Pen(color == PlayerColor.Red ? HumanColor : ComputerColor, 6f))
                {
                    pen.StartCap = pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, a, b);
                }
            }

            // Вершини з номерами 1..6
            using (var fill = new SolidBrush(Color.FromArgb(45, 52, 64)))
            using (var font = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                const float r = 17f;
                for (int i = 0; i < v.Length; i++)
                {
                    g.FillEllipse(fill, v[i].X - r, v[i].Y - r, 2 * r, 2 * r);
                    g.DrawString((i + 1).ToString(), font, Brushes.White, new RectangleF(v[i].X - r, v[i].Y - r, 2 * r, 2 * r), format);
                }
            }
        }

        // ------------------------------------------------------------------
        // Миша
        // ------------------------------------------------------------------

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int edge = InputEnabled ? FindEdgeAt(e.Location) : -1;
            if (edge != _hoverEdge)
            {
                _hoverEdge = edge;
                Cursor = edge >= 0 ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverEdge != -1)
            {
                _hoverEdge = -1;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!InputEnabled || e.Button != MouseButtons.Left) return;
            int edge = FindEdgeAt(e.Location);
            if (edge >= 0)
            {
                _hoverEdge = -1;
                EdgeClicked?.Invoke(this, edge);
            }
        }
    }
}
