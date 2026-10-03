using System;
using System.Collections.Generic;
using _Bludoku.Scripts.Blocks;
using UnityEngine;

namespace _Bludoku.Scripts.Boards
{
    public class Board : MonoBehaviour
    {
        private const int BoardSize = 9;
        private const int BoxSize = 3;
        private const int BoardCenter = (BoardSize - 1) / 2;
        private const int BoxCenter = (BoxSize - 1) / 2;
        public event Action<ClearResult> OnFigurePlaced;

        [SerializeField] private Transform tilesParent;

        private GridView _gridView;

        private int[,] _grid = new int[BoardSize, BoardSize];

        public bool IsInBounds(int x, int y) => x >= 0 && x < _grid.GetLength(1) && y >= 0 && y < _grid.GetLength(0);

        public int[,] GetGrid() => (int[,])_grid.Clone();

        public bool CanPlaceAnywhere(int[,] grid)
        {
            for (int y = 0; y <= _grid.GetLength(0) - grid.GetLength(0); y++)
            for (int x = 0; x <= _grid.GetLength(1) - grid.GetLength(1); x++)
                if (CanPlaceGrid(x, y, grid))
                    return true;
            return false;
        }

        private void Awake()
        {
            _gridView = new GridView(tilesParent);
            _gridView.Build(_grid);
        }

        public int SetFigure(Figure figure)
        {
            Vector2 corner = GetCornerPosition(figure);
            SetGrid((int)corner.x, (int)corner.y, figure.Grid);
            _gridView.UpdateGrid(_grid);

            ClearResult clearResult = CheckAndClear();
            clearResult.PlacementCenter = GetPlacementCenter(figure.Grid, (int)corner.x, (int)corner.y);
            OnFigurePlaced?.Invoke(clearResult);

            BoardSaveLoad.Save(_grid);

            return clearResult.ClearedCount;
        }

        public void SetCell(int x, int y, int value)
        {
            if (!IsInBounds(x, y)) return;
            _grid[y, x] = value;
        }

        public bool CanPlaceFigure(Figure figure)
        {
            Vector2 corner = GetCornerPosition(figure);

            return CanPlaceGrid((int)corner.x, (int)corner.y, figure.Grid);
        }

        public bool CanPlaceGrid(int x, int y, int[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == 0) continue;
                if (!IsInBounds(x + j, y + i)) return false;
                if (_grid[y + i, x + j] != 0) return false;
            }

            return true;
        }

        public void SetGrid(int x, int y, int[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == 0) continue;
                SetCell(x + j, y + i, grid[i, j]);
            }
        }

        public void UpdateHighlight(Figure figure)
        {
            _gridView.UpdateGrid(_grid);

            if (!CanPlaceFigure(figure)) return;

            Vector2 corner = GetCornerPosition(figure);
            int x = (int)corner.x;
            int y = (int)corner.y;

            int[,] simGrid = (int[,])_grid.Clone();
            for (int i = 0; i < figure.Grid.GetLength(0); i++)
            for (int j = 0; j < figure.Grid.GetLength(1); j++)
                if (figure.Grid[i, j] != 0)
                    simGrid[y + i, x + j] = figure.Grid[i, j];

            bool[,] willClear = BuildClearMask(simGrid, out var remove);

            for (int i = 0; i < figure.Grid.GetLength(0); i++)
            for (int j = 0; j < figure.Grid.GetLength(1); j++)
                if (figure.Grid[i, j] != 0)
                    _gridView.SetHighlight(y + i, x + j, HighlightType.Placement);

            for (int row = 0; row < BoardSize; row++)
            for (int col = 0; col < BoardSize; col++)
                if (willClear[row, col] && _grid[row, col] != 0)
                    _gridView.SetHighlight(row, col, HighlightType.WillClear);
        }

        public void ClearHighlight()
        {
            _gridView.UpdateGrid(_grid);
        }

        public ClearResult CheckAndClear()
        {
            var groupCenters = new List<Vector2Int>();
            bool[,] toClear = BuildClearMask(_grid, out var clearedGroups, groupCenters);
            var result = new ClearResult
            { 
                ClearedPositions = new List<Vector3>(),
                ClearedGroupsCount = clearedGroups
            };

            foreach (var center in groupCenters)
                result.ClearedGroupCenters.Add(GridToWorld(center.y, center.x));

            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    if (toClear[row, col])
                    {
                        result.ClearedPositions.Add(GridToWorld(row, col));
                        _grid[row, col] = 0;
                        result.ClearedCount++;
                    }
                }
            }

            if (result.ClearedCount > 0)
                _gridView.UpdateGrid(_grid);

            return result;
        }

        private Vector3 GridToWorld(int row, int col)
        {
            return transform.position + new Vector3(col - (_grid.GetLength(1) - 1) / 2f,
                -(row - (_grid.GetLength(0) - 1) / 2f), 0f);
        }

        private Vector3 GetPlacementCenter(int[,] figureGrid, int column, int row)
        {
            var bounds = new Bounds();
            bool hasCell = false;
            for (int y = 0; y < figureGrid.GetLength(0); y++)
            for (int x = 0; x < figureGrid.GetLength(1); x++)
            {
                if (figureGrid[y, x] == 0) continue;
                Vector3 position = GridToWorld(row + y, column + x);
                if (!hasCell)
                    bounds = new Bounds(position, Vector3.zero);
                else
                    bounds.Encapsulate(position);
                hasCell = true;
            }

            return hasCell ? bounds.center : transform.position;
        }

        public void LoadGrid()
        {
            _grid = BoardSaveLoad.TryLoad(out int[,] loadedGrid) ? loadedGrid : new int[BoardSize, BoardSize];
            if (_grid == null)
            {
                _grid = new int[BoardSize, BoardSize];
            }

            _gridView.UpdateGrid(_grid);
        }

        public void ResetBoard()
        {
            _grid = new int[BoardSize, BoardSize];
            _gridView.UpdateGrid(_grid);
        }

        private Vector2 GetCornerPosition(Figure figure)
        {
            Vector2 figurePosition = figure.transform.position - transform.position;

            int rows = figure.Grid.GetLength(0);
            int cols = figure.Grid.GetLength(1);

            float offsetX = (cols - 1) * -0.5f;
            float offsetY = (rows - 1) * 0.5f;

            int xPos = Mathf.RoundToInt(figurePosition.x + offsetX + (_grid.GetLength(0) - 1) / 2f);
            int yPos = Mathf.RoundToInt(figurePosition.y + offsetY - (_grid.GetLength(1) - 1) / 2f) * -1;

            return new Vector2(xPos, yPos);
        }

        private static bool[,] BuildClearMask(int[,] grid, out int clearedGroups,
            List<Vector2Int> groupCenters = null)
        {
            bool[,] mask = new bool[BoardSize, BoardSize];
            clearedGroups = 0;

            for (int row = 0; row < BoardSize; row++)
            {
                if (IsRowComplete(grid, row))
                {
                    MarkRow(mask, row);
                    clearedGroups++;
                    groupCenters?.Add(new Vector2Int(BoardCenter, row));
                }
            }

            for (int col = 0; col < BoardSize; col++)
            {
                if (IsColumnComplete(grid, col))
                {
                    MarkColumn(mask, col);
                    clearedGroups++;
                    groupCenters?.Add(new Vector2Int(col, BoardCenter));
                }
            }

            for (int boxRow = 0; boxRow < BoardSize / BoxSize; boxRow++)
            {
                for (int boxCol = 0; boxCol < BoardSize / BoxSize; boxCol++)
                {
                    if (IsBoxComplete(grid, boxRow, boxCol))
                    {
                        MarkBox(mask, boxRow, boxCol);
                        clearedGroups++;
                        groupCenters?.Add(new Vector2Int(boxCol * BoxSize + BoxCenter, boxRow * BoxSize + BoxCenter));
                    }
                }
            }

            return mask;
        }

        private static bool IsRowComplete(int[,] grid, int row)
        {
            for (int col = 0; col < BoardSize; col++)
                if (grid[row, col] == 0)
                    return false;
            return true;
        }

        private static bool IsColumnComplete(int[,] grid, int col)
        {
            for (int row = 0; row < BoardSize; row++)
                if (grid[row, col] == 0)
                    return false;
            return true;
        }

        private static bool IsBoxComplete(int[,] grid, int boxRow, int boxCol)
        {
            for (int i = boxRow * BoxSize; i < boxRow * BoxSize + BoxSize; i++)
            for (int j = boxCol * BoxSize; j < boxCol * BoxSize + BoxSize; j++)
                if (grid[i, j] == 0)
                    return false;
            return true;
        }

        private static void MarkRow(bool[,] mask, int row)
        {
            for (int col = 0; col < BoardSize; col++) mask[row, col] = true;
        }

        private static void MarkColumn(bool[,] mask, int col)
        {
            for (int row = 0; row < BoardSize; row++) mask[row, col] = true;
        }

        private static void MarkBox(bool[,] mask, int boxRow, int boxCol)
        {
            for (int i = boxRow * BoxSize; i < boxRow * BoxSize + BoxSize; i++)
            for (int j = boxCol * BoxSize; j < boxCol * BoxSize + BoxSize; j++)
                mask[i, j] = true;
        }
    }
}
