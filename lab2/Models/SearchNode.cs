namespace lab2.Models
{
    public class SearchNode
    {
        public PuzzleState State { get; }
        public SearchNode Parent { get; }
        public int G { get; }          // вартість від старту
        public int H { get; }          // евристика
        public int F => G + H;
        public string Action { get; }  

        public SearchNode(PuzzleState state, SearchNode parent = null, int g = 0, int h = 0, string action = null)
        {
            State = state;
            Parent = parent;
            G = g;
            H = h;
            Action = action;
        }
    }
}