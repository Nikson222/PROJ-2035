#if UNITY_EDITOR

using _Scripts.Game.Data;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Game.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        private LevelData _selectedLevelData;
        private LevelEditorController _controller;
        private DotData _editedDotData;
        private Dot _lastSelectedDot;

        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow() => GetWindow<LevelEditorWindow>("Level Editor");

        private void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);
            GUILayout.Space(5);

            _selectedLevelData = (LevelData)EditorGUILayout.ObjectField("Level Data", _selectedLevelData, typeof(LevelData), false);
            _controller = FindObjectOfType<LevelEditorController>();

            if (_controller == null)
            {
                EditorGUILayout.HelpBox("LevelEditorController not found in the scene!", MessageType.Warning);
                return;
            }

            GUILayout.Space(10);

            bool isPlaying = EditorApplication.isPlaying;
            GUI.enabled = _selectedLevelData != null && isPlaying;

            if (GUILayout.Button("Load Level into Scene"))
                _controller.LoadLevel(_selectedLevelData);

            if (GUILayout.Button("Save Scene to LevelData"))
                _controller.SaveLevelToConfig();
            
            if (GUILayout.Button("Clear All Dots"))
                _controller.ClearAllDots();

            if (GUILayout.Button("Clear Scene"))
                _controller.ClearScene();

            GUI.enabled = true;

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Selected Dot", EditorStyles.boldLabel);

            var selectedDot = _controller.SelectedDot;

            if (selectedDot != _lastSelectedDot)
            {
                _lastSelectedDot = selectedDot;
                _editedDotData = selectedDot ? selectedDot.GetDotData() : new DotData();
            }

            if (selectedDot != null)
            {
                GUILayout.Space(5);
                _editedDotData.ColorType = (DotColorType)EditorGUILayout.EnumPopup("Color", _editedDotData.ColorType);
                _editedDotData.IsStartDot = EditorGUILayout.Toggle("Is Start Dot", _editedDotData.IsStartDot);
                _editedDotData.IsUniversalDot = EditorGUILayout.Toggle("Is Universal Dot", _editedDotData.IsUniversalDot);
                _editedDotData.IsObstacle = EditorGUILayout.Toggle("Is Obstacle Dot", _editedDotData.IsObstacle);

                GUI.enabled = isPlaying;

                if (GUILayout.Button("Apply Changes"))
                    selectedDot.Setup(_editedDotData);

                if (GUILayout.Button("Delete Dot"))
                {
                    selectedDot.Cell.ClearDot();
                    _controller.DeleteDot(selectedDot);
                    DestroyImmediate(selectedDot.gameObject);
                    _controller.SelectDot(null);
                }

                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.HelpBox("No dot selected", MessageType.Info);
            }
        }
    }
}

#endif
