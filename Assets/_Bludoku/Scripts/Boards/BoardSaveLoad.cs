using System;
using System.IO;
using UnityEngine;

namespace _Bludoku.Scripts.Boards
{
    public static class BoardSaveLoad
    {
        private static string FilePath => Path.Combine(Application.persistentDataPath, "board.json");

        public static void Save(int[,] grid)
        {
            var data = new GridData { cells = Flatten(grid) };
            File.WriteAllText(FilePath, JsonUtility.ToJson(data));
        }

        public static bool TryLoad(out int[,] grid)
        {
            grid = null;
            if (!File.Exists(FilePath)) return false;

            try
            {
                var data = JsonUtility.FromJson<GridData>(File.ReadAllText(FilePath));
                grid = Unflatten(data.cells, 9, 9);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Delete()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        private static int[] Flatten(int[,] grid)
        {
            int rows = grid.GetLength(0), cols = grid.GetLength(1);
            int[] flat = new int[rows * cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    flat[r * cols + c] = grid[r, c];
            return flat;
        }

        private static int[,] Unflatten(int[] flat, int rows, int cols)
        {
            int[,] grid = new int[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    grid[r, c] = flat[r * cols + c];
            return grid;
        }

        [Serializable]
        private class GridData
        {
            public int[] cells;
        }
    }
}
