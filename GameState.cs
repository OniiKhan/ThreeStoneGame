using System;

namespace _3Tas
{
    public enum Player
    {
        None, Red, Blue
    }
    public class GameState
    {
        public event Action<int, int, int, int> MoveMade;
        public event Action GameEnded;
        public event Action GameRestarted;

        public Player[,] Board { get; private set; }
        public Player CurrentPlayer { get; set; }
        
        private short TurnPassed { get; set; }
        private bool Selection { get; set; }
        public bool GameOver { get; private set; }
        private int PosR { get; set; }
        private int PosC { get; set; }

        public GameState()
        {
            Board = new Player[3, 3];
            CurrentPlayer = Player.Red;
            TurnPassed = 0;
            Selection = false;
            GameOver = false;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.Red ? Player.Blue : Player.Red;
        }
        private bool IsEmpty(int r, int c)
        {
            if (Board[r, c] == Player.None) 
            {
                return true;
            }
            return false;
        }
        private bool IsYours(int r, int c)
        {
            if (Board[r,c] == CurrentPlayer)
            {
                return true;
            }
            return false;
        }
        private bool IsNear(int r, int c)
        {
            if ((PosC == c && Math.Abs(PosR - r) == 1) || (PosR == r && Math.Abs(PosC - c) == 1)) 
            {
                return true;
            }
            return false;
        }
        private bool IsLocked(int r, int c)
        {
            if (Board[r, c == 0 ? 0 : c - 1] == Player.None ||
                Board[r, c == 2 ? 2 : c + 1] == Player.None ||
                Board[r == 0 ? 0 : r - 1, c] == Player.None ||
                Board[r == 2 ? 2 : r + 1, c] == Player.None)
            {
                return false;
            }
            return true;
        }
        private bool AreSquaresMarked((int, int)[] squares, Player player)
        {
            foreach ((int r, int c) in squares)
            {
                if (Board[r, c] != player)
                {
                    return false;
                }
            }

            return true;
        }
        private bool DidMoveWin(int r, int c)
        {
            (int, int)[] row = new[] { (r, 0), (r, 1), (r, 2) };
            (int, int)[] col = new[] { (0, c), (1, c), (2, c) };
            if (AreSquaresMarked(row, CurrentPlayer) || AreSquaresMarked(col, CurrentPlayer))
            {
                GameOver = true;
                return true;
            }
            return false;
        }
        private void PlaceStone(int r, int c)
        {
            if (IsEmpty(r, c))
            {
                Board[r, c] = CurrentPlayer;
                
                TurnPassed++;
                MoveMade?.Invoke(-1, 0, r, c);
                if (DidMoveWin(r, c))
                {
                    GameEnded?.Invoke();
                    return;
                }
                SwitchPlayer();
            }
        }
        private void SelectStone(int r, int c)
        {
            if (IsYours(r, c) && !IsLocked(r, c))
            {
                PosR = r;
                PosC = c;
                Selection = true;
                MoveMade?.Invoke(0, -1, r, c);
            }
        }
        private void SelectPlace(int r, int c)
        {
            if (PosR == r && PosC == c)
            {
                Selection = false;
                MoveMade?.Invoke(-1, -1, r, c);
            }
            else if (IsEmpty(r, c) && IsNear(r, c))
            {
                Board[r, c] = CurrentPlayer;
                Board[PosR, PosC] = Player.None;
                
                Selection = false;
                MoveMade?.Invoke(PosR, PosC, r, c);
                if (DidMoveWin(r, c))
                {
                    GameEnded?.Invoke();
                    return;
                }
                SwitchPlayer();
            }
        }
        public void MakeMove(int r, int c)
        {
            if (TurnPassed < 6)
            {
                PlaceStone(r, c);
            }
            else if (!Selection)
            {
                SelectStone(r, c);
            }
            else
            {
                SelectPlace(r, c);
            }
        }
        public void Reset()
        {
            Board = new Player[3, 3];
            CurrentPlayer = Player.Red;
            TurnPassed = 0;
            GameOver = true;
            GameRestarted?.Invoke();
        }
    }
}