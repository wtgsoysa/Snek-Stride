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
        public GrideValue[,] GrideValues { get; }

        public bool GameOver { get; private set; }

        public int Score { get; private set; }
        public Direction Dir { get; private set; }

        private readonly LinkedList<Position> snakePosition = new LinkedList<Position>();   
        private readonly Random random = new Random();

        public GameState(int rows,int cols)
        {
            Rows = rows;
            Cols = cols;
            GrideValues = new GrideValue[rows, cols];
            Dir = Direction.Right;


            AddSnake();
            AddFood();


        }

        private void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c<= Cols; c++)
            {
                GrideValues[r, c] = GrideValue.Snake;
                snakePosition.AddFirst(new Position(r, c));
            }

        }

        private IEnumerable<Position> EmptyPositions()
        {
            for(int r=0; r< Rows; r++)
            {
                for(int c=0; c< Cols; c++)
                {
                    if (GrideValues[r,c] == GrideValue.Empty)
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
            GrideValues[pos.Row, pos.Col] = GrideValue.Food;
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
            GrideValues[pos.Row, pos.Col] = GrideValue.Snake;
            
        }

        private void RemoveTail()
        {
            Position tail = snakePosition.Last.Value;
            GrideValues[tail.Row, tail.Col] = GrideValue.Empty;
            snakePosition.RemoveLast();
        }

        public void ChangeDirection(Direction dir)
        {
            Dir = dir;
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

            return GrideValues [newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
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
