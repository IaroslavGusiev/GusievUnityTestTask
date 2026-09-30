using UnityEngine;

namespace _Bludoku.Scripts.MainMenu
{
public static class SaveSystem
{
    private const string KEY_LEVEL = "Save_LevelIndex";

    public static int CurrentLevelIndex => PlayerPrefs.GetInt(KEY_LEVEL, 0);

    public static int CurrentLevelNumber => CurrentLevelIndex + 1;

    public static void SaveLevel(int index)
    {
        PlayerPrefs.SetInt(KEY_LEVEL, index);
        PlayerPrefs.Save();
    }

    public static void AdvanceLevel(int totalLevels)
    {
        if (totalLevels <= 0) return;
        int next = (CurrentLevelIndex + 1) % totalLevels;
        SaveLevel(next);
    }
}
}
