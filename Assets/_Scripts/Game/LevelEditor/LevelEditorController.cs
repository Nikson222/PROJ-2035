#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using _Scripts.Game.Data;
using UnityEditor;

namespace _Scripts.Game.LevelEditor
{
    public class LevelEditorController : MonoBehaviour
    {
        [SerializeField] private EditorGridManager _gridManager;

        private readonly List<Dot> _spawnedDots = new();
        private LevelData _loadedData;
        private Dot _selectedDot;
        public Dot SelectedDot => _selectedDot;

        private void Start()
        {
            _gridManager.Initialize(this);
            _gridManager.OnDotSpawned += AddDot;
        }

        public void LoadLevel(LevelData levelData)
        {
            if (levelData == null) return;

            _loadedData = levelData;

            _gridManager.CreateGrid(levelData.GridWidth, levelData.GridHeight);
            _gridManager.PlaceDots(levelData.Dots);

            _spawnedDots.Clear();
            _spawnedDots.AddRange(_gridManager.GetAllDots());
        }

        public void SaveLevelToConfig()
        {
            if (_loadedData == null) return;

            _loadedData.Dots.Clear();

            foreach (var dot in _spawnedDots)
                _loadedData.Dots.Add(dot.GetDotData());

            EditorUtility.SetDirty(_loadedData);
            Debug.Log("Level data saved.");
        }
        
        public void ClearAllDots()
        {
            foreach (var dot in new List<Dot>(_spawnedDots))
            {
                if (dot != null)
                {
                    dot.Cell?.ClearDot();
                    DestroyImmediate(dot.gameObject);
                }
            }

            _spawnedDots.Clear();
            _selectedDot = null;
            EditorWindow.GetWindow<LevelEditorWindow>()?.Repaint();
        }
        
        public void ClearScene()
        {
            _gridManager.ClearGrid();
            _spawnedDots.Clear();
        }

        public void SelectDot(Dot dot)
        {
            _selectedDot = dot;
            EditorWindow.GetWindow<LevelEditorWindow>()?.Repaint();
        }

        public void DeleteDot(Dot dot)
        {
            _spawnedDots.Remove(dot);
        }

        private void AddDot(Dot dot)
        {
            _spawnedDots.Add(dot);
        }

        private void OnDestroy()
        {
            _gridManager.OnDotSpawned -= AddDot;
        }
    }
}

#endif