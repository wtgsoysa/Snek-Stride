using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace snake
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GrideValue, ImageSource> gridValToImage = new()
        {
            {GrideValue.Empty,Images.Empty},
            {GrideValue.Snake,Images.Body},
            {GrideValue.Food,Images.Food},
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up, 0},
            {Direction.Right, 90},
            {Direction.Down, 180 },
            {Direction.Left,270 }

        };


        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImage;
        private GameState gameState;
        private bool gameRunning;


        public MainWindow()
        {
            InitializeComponent();
            gridImage = setupGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await showGameOver();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] setupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GrideValue gridVal = gameState.Grid[r, c]; // Corrected property name
                    gridImage[r, c].Source = gridValToImage[gridVal];
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPostion();
            Image image = gridImage[headPos.Row, headPos.Col];
            image.Source = Images.Head;

        }


        private async Task ShowCountDown()
        {
            for (int i = 0; i > 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }






        private async Task showGameOver()
        {
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }



    }
}
