using _Bludoku.Scripts.Blocks;
using UnityEngine;

namespace _Bludoku.Scripts.Core
{
    public static class StaticData
    {
        private static Figure _figurePrefab;
        private static Tile _tileOldPrefab;

        public static Figure GetFigurePrefab()
        {
            if (_figurePrefab != null)
            {
                return _figurePrefab;
            }
            
            _figurePrefab = Resources.Load<Figure>("Prefabs/Figure");
            return _figurePrefab;
        }
        
        public static Tile GetTilePrefab()
        {
            if (_tileOldPrefab != null)
            {
                return _tileOldPrefab;
            }
            
            _tileOldPrefab = Resources.Load<Tile>("Prefabs/Tile");
            return _tileOldPrefab;
        }
    }
}