using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace snake
{
    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GrideValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

       
        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePosition = new LinkedList<Position>();   
        private readonly Random random = new Random();

        public GameState(int rows,int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GrideValue[rows, cols];
            Dir = Direction.Right;


            AddSnake();
            AddFood();


        }

        private void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 0; c < 3; c++)
            {
                Grid[r, c] = GrideValue.Snake;
                snakePosition.AddFirst(new Position(r, c));
            }
        }


        private IEnumerable<Position> EmptyPositions()
        {
            for(int r=0; r< Rows; r++)
            {
                for(int c=0; c< Cols; c++)
                {
                    if (Grid[r,c] == GrideValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GrideValue.Food;
        }

        public Position HeadPostion()
        {
            return snakePosition.First();
        }

        public Position TailPosition()
        {
            return snakePosition.Last.Value;
        }

        public IEnumerable<Position> SnakePosition()
        {
            return snakePosition;
        }

        public void AddHead(Position pos)
        {
            snakePosition.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GrideValue.Snake;
            
        }

        private void RemoveTail()
        {
            Position tail = snakePosition.Last.Value;
            Grid[tail.Row, tail.Col] = GrideValue.Empty;
            snakePosition.RemoveLast();
        }

        private Direction GetLastDirection()
        {
            if(dirChanges.Count == 0)
            {
                return Dir;
            }

            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if(dirChanges.Count == 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();

        }

        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        private GrideValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GrideValue.Outside;
            }

            if (newHeadPos == TailPosition())
            {
                return GrideValue.Empty;
            }

            return Grid [newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
            if(dirChanges.Count > 0)
            {
                Dir = dirChanges.Last.Value;
                dirChanges.RemoveFirst();
            }

            Position newHeadPos = HeadPostion().Translate(Dir);
            GrideValue hit = WillHit(newHeadPos);

            if (hit == GrideValue.Outside || hit == GrideValue.Snake)
            {
                GameOver = true;
            }
            else if(hit == GrideValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if( hit == GrideValue.Food)
            {
                AddHead(newHeadPos);
                Score++ ;
                AddFood();
            }
        }
    }
}
