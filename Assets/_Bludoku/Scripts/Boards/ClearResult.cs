using UnityEngine;
using System.Collections.Generic;

namespace _Bludoku.Scripts.Boards
{
    public class ClearResult
    {
        public int ClearedCount;
        public int ClearedGroupsCount;
        public Vector3 PlacementCenter;
        public List<Vector3> ClearedPositions;
        public List<Vector3> ClearedGroupCenters = new();

        public Vector3 EffectCenter
        {
            get
            {
                if (ClearedGroupCenters == null || ClearedGroupCenters.Count == 0)
                    return PlacementCenter;

                return FindClosestGroupCenter();
            }
        }

        private Vector3 FindClosestGroupCenter()
        {
            Vector3 closest = ClearedGroupCenters[0];
            float closestDistance = (closest - PlacementCenter).sqrMagnitude;
            for (int i = 1; i < ClearedGroupCenters.Count; i++)
            {
                Vector3 center = ClearedGroupCenters[i];
                float distance = (center - PlacementCenter).sqrMagnitude;
                if (distance >= closestDistance) continue;
                closest = center;
                closestDistance = distance;
            }
            return closest;
        }
    }
}
