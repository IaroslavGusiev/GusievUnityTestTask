using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Bludoku.Scripts.Blocks
{
    public class FiguresSaveLoad
    {
        private const string FiguresSaveKey = "FiguresSave";
        
        public void SaveFigures(List<Figure> figures)
        {
            string saveData = "";
            foreach (var figure in figures)
            {
                if (figure != null)
                {
                    saveData += figure.ID + ";";
                }
                else
                {
                    saveData += "-1;";
                }
            }

            PlayerPrefs.SetString(FiguresSaveKey, saveData);
        }

        public int[] LoadFigures()
        {
            string saveData = PlayerPrefs.GetString(FiguresSaveKey, "");
            if (string.IsNullOrEmpty(saveData))
                return Array.Empty<int>();

            string[] figureIds = saveData.Split(';');
            int[] result = new int[figureIds.Length];
            for (int i = 0; i < figureIds.Length; i++)
            {
                if (int.TryParse(figureIds[i], out int id))
                {
                    result[i] = id;
                }
                else
                {
                    result[i] = -1;
                }
                Debug.Log($"Loaded figure ID: {result[i]} at position {i}");
            }

            return result;
        }
    }
}