using System.Collections.Generic;
using UnityEngine;

namespace _Bludoku.Scripts.Boards
{
    public class ClearResult
    {
        public int ClearedCount;
        public int FiguresRemovedCount;
        public List<Vector3> ClearedPositions;
    }
}
