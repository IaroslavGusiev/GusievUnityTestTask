using System;
using System.Collections.Generic;
using _Bludoku.Scripts.Save;

namespace _Bludoku.Scripts.Blocks
{
    public class FiguresSaveLoad
    {
        public static void SaveFigures(List<Figure> figures, int slotCount)
        {
            int[] ids = CreateEmptySlots(slotCount);

            foreach (Figure figure in figures)
            {
                ids[figure.SlotIndex] = figure.ID;
            }

            JsonSaveStorage.Save(SaveFiles.Figures, new FiguresData { ids = ids });
        }

        public static int[] LoadFigures(int slotCount)
        {
            if (JsonSaveStorage.TryLoad(SaveFiles.Figures, out FiguresData data) == false ||
                HasValidFigures(data.ids, slotCount) == false)
            {
                return Array.Empty<int>();
            }

            return data.ids;
        }

        private static int[] CreateEmptySlots(int slotCount)
        {
            var ids = new int[slotCount];

            for (var i = 0; i < ids.Length; i++)
            {
                ids[i] = -1;
            }

            return ids;
        }

        private static bool HasValidFigures(int[] ids, int slotCount)
        {
            if (ids == null || ids.Length != slotCount)
            {
                return false;
            }

            var hasFigure = false;

            foreach (int id in ids)
            {
                if (id < -1 || id >= FigureFactory.Shapes.Length)
                {
                    return false;
                }

                hasFigure |= id >= 0;
            }

            return hasFigure;
        }

        [Serializable]
        private class FiguresData
        {
            public int[] ids;
        }
    }
}
