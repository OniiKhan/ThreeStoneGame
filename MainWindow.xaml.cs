using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _3Tas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Player, ImageSource> imageSources = new()
        {
            { Player.Red, new BitmapImage(new Uri("pack://application:,,,/Assets/Red.png")) },
            { Player.Blue, new BitmapImage(new Uri("pack://application:,,,/Assets/Blu.png")) },
            { Player.None, new BitmapImage()}
        };

        private readonly GameState GameState = new();
        private readonly Image[,] imageControls = new Image[3, 3];
        public MainWindow()
        {
            InitializeComponent();
            SetupGameGrid();

            GameState.MoveMade += OnMoveMade;
            GameState.GameEnded += OnGameEnded;
            GameState.GameRestarted += OnGameRestarted;
        }

        private  void OnGameRestarted()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    imageControls[r, c].Source = null;
                }
            }
            EndScreen.Visibility = Visibility.Hidden;
        }

        private async void OnGameEnded()
        {
            await Task.Delay(1000);
            EndScreen.Visibility = Visibility.Visible;
            WinnerImage.Source = imageSources[GameState.CurrentPlayer];
        }

        private void SetupGameGrid()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Image imageControl = new();
                    GameGrid.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
        }

        private void OnMoveMade(int r0, int c0, int r, int c)
        {
            if (r0 == -1 && c0 == -1)
            {
                imageControls[r, c].Opacity = 1;
                return;
            }
            if (c0 == -1)
            {
                imageControls[r, c].Opacity = 0.5;
                return;
            }
            if (r0 != -1)
            {
                imageControls[r0, c0].Source = imageSources[Player.None];
                imageControls[r0, c0].Opacity = 1;
            }
            Player player = GameState.Board[r, c];
            imageControls[r, c].Source = imageSources[player];
        }

        private void GameGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double squareSize = GameGrid.Width / 3;
            Point click = e.GetPosition(GameGrid);
            int row = (int)(click.Y / squareSize);
            int col = (int)(click.X / squareSize);
            GameState.MakeMove(row, col);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (GameState.GameOver)
            {
                GameState.Reset();
            }
        }
    }
}
