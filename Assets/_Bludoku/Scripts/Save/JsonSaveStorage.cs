using System;
using System.IO;
using UnityEngine;

namespace _Bludoku.Scripts.Save
{
    public static class JsonSaveStorage
    {
        public static void Save<T>(string fileName, T data)
        {
            try
            {
                string json = JsonUtility.ToJson(data);
                Directory.CreateDirectory(Application.persistentDataPath);
                File.WriteAllText(GetPath(fileName), json);
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                LogFailure(fileName, "save", exception);
            }
        }

        public static bool TryLoad<T>(string fileName, out T data) where T : class
        {
            data = null;
            string path = GetPath(fileName);

            if (File.Exists(path) == false)
            {
                return false;
            }

            try
            {
                data = JsonUtility.FromJson<T>(File.ReadAllText(path));
                return data != null;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                LogFailure(fileName, "load", exception);
                return false;
            }
        }

        public static bool Delete(string fileName)
        {
            try
            {
                File.Delete(GetPath(fileName));
                return true;
            }
            catch (Exception exception) when (IsStorageException(exception))
            {
                LogFailure(fileName, "delete", exception);
                return false;
            }
        }

        private static string GetPath(string fileName) =>
            Path.Combine(Application.persistentDataPath, fileName);

        private static bool IsStorageException(Exception exception) =>
            exception is IOException or UnauthorizedAccessException or ArgumentException;

        private static void LogFailure(string fileName, string operation, Exception exception) =>
            Debug.Log($"[Save] Could not {operation} {fileName}: {exception.Message}");
    }
}
