using System;

namespace Tetris.Model
{
    class TetrisGrid
    {
        public const int gridColumns = 10;
        public const int gridRows = 20;
        private static int[,] grid = new int[gridRows, gridColumns];
        private static int[,] grid2 = new int[gridRows, gridColumns];
        
        public TetrisGrid()
        {           
        }

        public int[,] Grid1
        {
            get { return grid; }
            set { grid = value; }
        }

        public int[,] Grid2
        {
            get { return grid2; }
            set { grid2 = value; }
        }

        public void EmptyGrid()
        {
            for (int i = 0; i < gridRows; i++)
                for (int j = 0; j < gridColumns; j++)
                    grid[i, j] = 0;
        }

        public void EmptyGrid2()
        {
            for (int i = 0; i < gridRows; i++)
                for (int j = 0; j < gridColumns; j++)
                    grid2[i, j] = 0;
        }
    }
}
