using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public class Layer
    {
        private readonly CellType[,] _cells;

        public int Width { get; }
        public int Height { get; }
        public int Measure => Width * Height;

        public Layer(int width, int height)
        {
            if (width < 5 || height < 5 || width % 2 == 0 || height % 2 == 0)
            {
                throw new ArgumentException("縦横共に5マス以上、奇数にしてください。");
            }

            Width = width;
            Height = height;
            _cells = new CellType[height, width];
        }

        public CellType Get(Vector2 position)
        {
            if (IsOutOfRange(position))
            {
                return CellType.OutOfRange;
            }

            return _cells[(int)position.Y, (int)position.X];
        }

        public void Set(Vector2 position, CellType chip)
        {
            if (IsOutOfRange(position))
            {
                return;
            }

            _cells[(int)position.Y, (int)position.X] = chip;
        }

        internal bool IsOutOfRange(Vector2 position)
        {
            if (position.X < 0 || Width <= position.X)
            {
                return true;
            }
            if (position.Y < 0 || Height <= position.Y)
            {
                return true;
            }

            return false;
        }

        internal void BorderValues(CellType chip)
        {
            for (int i = 0; i < Width; i++)
            {
                _cells[0, i] = chip;
                _cells[Height - 1, i] = chip;
            }
            for (int i = 0; i < Height; i++)
            {
                _cells[i, 0] = chip;
                _cells[i, Width - 1] = chip;
            }
        }

        internal void FillValues(CellType chip)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _cells[y, x] = chip;
                }
            }
        }
    }
}
