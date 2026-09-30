using _Bludoku.Scripts.Core;
using UnityEngine;

namespace _Bludoku.Scripts.Blocks
{
    public static class FigureFactory
    {
        public static readonly int[][,] Shapes =
        {
            // 1x1
            new int[,] { { 1 } },

            // Lines
            new int[,] { { 1, 1 } },
            new int[,] { { 1 }, 
                        { 1 } },
            new int[,] { { 1, 1, 1 } },
            new int[,] { { 1 },
                        { 1 },
                        { 1 } },
            new int[,] { { 1, 1, 1, 1 } },
            new int[,] { { 1 }, 
                        { 1 },
                        { 1 },
                        { 1 } },
            new int[,] { { 1, 1, 1, 1, 1 } },
            new int[,] { { 1 },
                        { 1 },
                        { 1 },
                        { 1 },
                        { 1 } },

            // Squares
            new int[,] { { 1, 1 },
                        { 1, 1 } },
            new int[,] { { 1, 1, 1 }, 
                        { 1, 1, 1 }, 
                        { 1, 1, 1 } },

            // L shapes
            new int[,] { { 1, 0 }, { 1, 0 }, { 1, 1 } },
            new int[,] { { 0, 1 }, { 0, 1 }, { 1, 1 } },
            new int[,] { { 1, 1 }, { 1, 0 }, { 1, 0 } },
            new int[,] { { 1, 1 }, { 0, 1 }, { 0, 1 } },

            // T shapes
            new int[,] { { 1, 1, 1 }, { 0, 1, 0 } },
            new int[,] { { 0, 1, 0 }, { 1, 1, 1 } },
            new int[,] { { 1, 0 }, { 1, 1 }, { 1, 0 } },
            new int[,] { { 0, 1 }, { 1, 1 }, { 0, 1 } },

            // S / Z shapes
            new int[,] { { 0, 1, 1 }, { 1, 1, 0 } },
            new int[,] { { 1, 1, 0 }, { 0, 1, 1 } },
            new int[,] { { 1, 0 }, { 1, 1 }, { 0, 1 } },
            new int[,] { { 0, 1 }, { 1, 1 }, { 1, 0 } },

            // Corners
            new int[,] { { 1, 0 }, { 1, 1 } },
            new int[,] { { 0, 1 }, { 1, 1 } },
            new int[,] { { 1, 1 }, { 1, 0 } },
            new int[,] { { 1, 1 }, { 0, 1 } },

            // Big L
            new int[,] { { 1, 0, 0 }, { 1, 0, 0 }, { 1, 1, 1 } },
            new int[,] { { 0, 0, 1 }, { 0, 0, 1 }, { 1, 1, 1 } },
            new int[,] { { 1, 1, 1 }, { 1, 0, 0 }, { 1, 0, 0 } },
            new int[,] { { 1, 1, 1 }, { 0, 0, 1 }, { 0, 0, 1 } },
        };
        
        private static readonly System.Random Rng = new();

        public static Figure GetRandom()
        {
            return GetByIndex(Rng.Next(Shapes.Length));
        }

        public static Figure GetByIndex(int index)
        {
            Figure figure = Object.Instantiate(StaticData.GetFigurePrefab());
            figure.Initialize(Shapes[index],  index);
            return figure;
        }

        public static int[,] CreateGrid(int[,] grid, int value)
        {
            int[,] filledGrid = new int[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                filledGrid[i, j] = grid[i, j] != 0 ? value : 0;
            return filledGrid;
        }
    }
}