using System.Collections.Generic;
using UnityEngine;

namespace _Bludoku.Scripts.Core
{
    public static class FigureSuggestion
    {
        private const int ClearBonus = 50;

        public static List<int> SelectShapeIndices(int[,] boardGrid, int[][,] shapes, int count, int minTotalCells = 0)
        {
            // Fill counts are the same for all shapes — compute once.
            ComputeFills(boardGrid, out int[] rowFill, out int[] colFill, out int[,] boxFill);

            var scored = new List<(int index, int score)>(shapes.Length);

            for (int i = 0; i < shapes.Length; i++)
            {
                int score = BestPlacementScore(boardGrid, rowFill, colFill, boxFill, shapes[i]);
                if (score >= 0)
                    scored.Add((i, score));
            }

            if (scored.Count == 0)
                return new List<int>();

            scored.Sort((a, b) => b.score.CompareTo(a.score));

            // Randomly pick from the top half so results vary between rounds,
            // while still preferring shapes that fit the board well.
            int topHalf = Mathf.Max(count, (scored.Count + 1) / 2);
            int poolSize = Mathf.Min(topHalf, scored.Count);

            ShuffleRange(scored, poolSize);

            return PickWithMinCells(scored, shapes, count, minTotalCells);
        }

        // Picks count shapes from pool. If their total cell count falls below minTotalCells,
        // swaps out the smallest selected shapes with larger ones from the remaining pool.
        private static List<int> PickWithMinCells(List<(int index, int score)> pool, int[][,] shapes, int count, int minTotalCells)
        {
            var result = new List<int>(count);
            for (int i = 0; i < count && i < pool.Count; i++)
                result.Add(pool[i].index);

            if (minTotalCells <= 0 || TotalCells(result, shapes) >= minTotalCells)
                return result;

            // Replace the smallest selected shape with a larger one from the remaining pool.
            for (int pi = count; pi < pool.Count; pi++)
            {
                int candidateIndex = pool[pi].index;
                int candidateCells = CountCells(shapes[candidateIndex]);

                int worstPos = SmallestShapePosition(result, shapes);
                if (candidateCells <= CountCells(shapes[result[worstPos]])) continue;

                result[worstPos] = candidateIndex;

                if (TotalCells(result, shapes) >= minTotalCells) break;
            }

            return result;
        }

        private static int SmallestShapePosition(List<int> indices, int[][,] shapes)
        {
            int worstPos = 0;
            for (int i = 1; i < indices.Count; i++)
                if (CountCells(shapes[indices[i]]) < CountCells(shapes[indices[worstPos]]))
                    worstPos = i;
            return worstPos;
        }

        private static int TotalCells(List<int> indices, int[][,] shapes)
        {
            int total = 0;
            foreach (int i in indices)
                total += CountCells(shapes[i]);
            return total;
        }

        private static int CountCells(int[,] shape)
        {
            int count = 0;
            for (int i = 0; i < shape.GetLength(0); i++)
                for (int j = 0; j < shape.GetLength(1); j++)
                    if (shape[i, j] != 0) count++;
            return count;
        }

        private static void ComputeFills(int[,] boardGrid, out int[] rowFill, out int[] colFill, out int[,] boxFill)
        {
            rowFill = new int[9];
            colFill = new int[9];
            boxFill = new int[3, 3];

            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (boardGrid[r, c] != 0)
                    {
                        rowFill[r]++;
                        colFill[c]++;
                        boxFill[r / 3, c / 3]++;
                    }
        }

        // Returns the best score for placing this shape anywhere on the board.
        // Returns -1 if the shape cannot be placed anywhere.
        private static int BestPlacementScore(int[,] boardGrid, int[] rowFill, int[] colFill, int[,] boxFill, int[,] shape)
        {
            int best = -1;
            int limitY = boardGrid.GetLength(0) - shape.GetLength(0);
            int limitX = boardGrid.GetLength(1) - shape.GetLength(1);

            for (int y = 0; y <= limitY; y++)
                for (int x = 0; x <= limitX; x++)
                {
                    if (!CanPlace(boardGrid, shape, x, y)) continue;
                    int score = ScorePlacement(rowFill, colFill, boxFill, shape, x, y);
                    if (score > best) best = score;
                }

            return best;
        }

        private static bool CanPlace(int[,] boardGrid, int[,] shape, int x, int y)
        {
            for (int i = 0; i < shape.GetLength(0); i++)
                for (int j = 0; j < shape.GetLength(1); j++)
                    if (shape[i, j] != 0 && boardGrid[y + i, x + j] != 0)
                        return false;
            return true;
        }

        // Scores a placement by counting how many cells each row, column, and box
        // would have after placing the shape. No grid clone needed — counts are additive.
        private static int ScorePlacement(int[] rowFill, int[] colFill, int[,] boxFill, int[,] shape, int x, int y)
        {
            int[] rowAdd = new int[9];
            int[] colAdd = new int[9];
            int[,] boxAdd = new int[3, 3];

            for (int i = 0; i < shape.GetLength(0); i++)
                for (int j = 0; j < shape.GetLength(1); j++)
                    if (shape[i, j] != 0)
                    {
                        int r = y + i, c = x + j;
                        rowAdd[r]++;
                        colAdd[c]++;
                        boxAdd[r / 3, c / 3]++;
                    }

            int score = 0;

            for (int r = 0; r < 9; r++)
                score += Segment(rowFill[r] + rowAdd[r]);

            for (int c = 0; c < 9; c++)
                score += Segment(colFill[c] + colAdd[c]);

            for (int br = 0; br < 3; br++)
                for (int bc = 0; bc < 3; bc++)
                    score += Segment(boxFill[br, bc] + boxAdd[br, bc]);

            return score;
        }

        private static int Segment(int filled) => filled == 9 ? ClearBonus : filled;

        private static void ShuffleRange<T>(List<T> list, int count)
        {
            for (int i = count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
