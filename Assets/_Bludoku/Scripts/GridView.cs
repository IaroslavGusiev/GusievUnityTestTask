using _Bludoku.Scripts.Blocks;
using _Bludoku.Scripts.Core;
using UnityEngine;

namespace _Bludoku.Scripts
{
    public enum HighlightType { Placement = 2, WillClear = 3 }

    public class GridView
    {
        private readonly Transform _parent;
        private readonly float _tileSize;

        private Tile[,] _tiles;

        public GridView(Transform parent, float tileSize = 1f)
        {
            _parent = parent;
            _tileSize = tileSize;
        }

        public void Build(int[,] grid)
        {
            Clear();

            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            _tiles = new Tile[rows, cols];

            float offsetX = (cols - 1) * _tileSize * 0.5f;
            float offsetY = (rows - 1) * _tileSize * 0.5f;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Tile tile = Object.Instantiate(StaticData.GetTilePrefab(), _parent);
                    tile.transform.localPosition = new Vector3(j * _tileSize - offsetX, -i * _tileSize + offsetY, 0f);
                    tile.SetState(grid[i, j]);
                    _tiles[i, j] = tile;
                }
            }
        }

        public void SetAlpha(float alpha)
        {
            foreach (var tile in _tiles)
            {
                tile.SetAlpha(alpha);
            }
        }

        public void SetHighlight(int row, int col, HighlightType type)
        {
            _tiles[row, col].SetState((int)type);
        }

        public void UpdateGrid(int[,] grid)
        {
            for (int i = 0; i < _tiles.GetLength(0); i++)
            for (int j = 0; j < _tiles.GetLength(1); j++)
            {
                _tiles[i, j].SetState(grid[i, j]);
            }
        }

        private void Clear()
        {
            foreach (Transform child in _parent)
                Object.Destroy(child.gameObject);
            _tiles = null;
        }
    }
}
