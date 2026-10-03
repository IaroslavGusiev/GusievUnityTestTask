using _Bludoku.Scripts.Save;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Bludoku.Editor
{
    public class HotActionsWindow : EditorWindow
    {
        private const string LayoutPath = "Assets/_Bludoku/Editor/HotActionsWindow.uxml";

        private Button _clearButton;
        private Label _status;

        [MenuItem("Tools/Bludoku/Hot Actions")]
        public static void Open() =>
            GetWindow<HotActionsWindow>("Hot Actions");

        private void OnEnable() =>
            EditorApplication.playModeStateChanged += HandlePlayModeChanged;

        private void OnDisable() =>
            EditorApplication.playModeStateChanged -= HandlePlayModeChanged;

        public void CreateGUI()
        {
            VisualTreeAsset layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LayoutPath);
            layout.CloneTree(rootVisualElement);

            rootVisualElement.Q<TextField>("save-path").SetValueWithoutNotify(Application.persistentDataPath);
            _clearButton = rootVisualElement.Q<Button>("clear-saves");
            _status = rootVisualElement.Q<Label>("status");
            _clearButton.clicked += ClearSaveFiles;

            RefreshAvailability();
        }

        private void HandlePlayModeChanged(PlayModeStateChange state) =>
            RefreshAvailability();

        private void RefreshAvailability() =>
            _clearButton?.SetEnabled(EditorApplication.isPlayingOrWillChangePlaymode == false);

        private void ClearSaveFiles()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            bool cleared = JsonSaveStorage.Delete(SaveFiles.Board);
            cleared &= JsonSaveStorage.Delete(SaveFiles.Figures);
            cleared &= JsonSaveStorage.Delete(SaveFiles.Score);

            _status.text = cleared
                ? "Save files cleared. The next game will start fresh."
                : "Some files could not be cleared. Check the Console.";
        }
    }
}
