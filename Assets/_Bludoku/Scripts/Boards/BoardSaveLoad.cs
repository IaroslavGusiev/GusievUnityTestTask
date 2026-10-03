using System;
using _Bludoku.Scripts.Save;

namespace _Bludoku.Scripts.Boards
{
    public static class BoardSaveLoad
    {
        private const int BoardSize = 9;

        public static void Save(int[,] grid)
        {
            var data = new GridData { cells = Flatten(grid) };
            JsonSaveStorage.Save(SaveFiles.Board, data);
        }

        public static bool TryLoad(out int[,] grid)
        {
            grid = null;

            if (JsonSaveStorage.TryLoad(SaveFiles.Board, out GridData data) == false ||
                data.cells == null || data.cells.Length != BoardSize * BoardSize)
            {
                return false;
            }

            grid = Unflatten(data.cells, BoardSize, BoardSize);
            return true;
        }

        public static void Delete() =>
            JsonSaveStorage.Delete(SaveFiles.Board);

        private static int[] Flatten(int[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int[] flat = new int[rows * cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    flat[r * cols + c] = grid[r, c];
                }
            }
            return flat;
        }

        private static int[,] Unflatten(int[] flat, int rows, int cols)
        {
            int[,] grid = new int[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    grid[r, c] = flat[r * cols + c];
                }
            }
            return grid;
        }

        [Serializable]
        private class GridData
        {
            public int[] cells;
        }
    }
}
