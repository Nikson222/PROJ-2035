#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Game.LevelEditor
{
    public class GridCellClickHandler : MonoBehaviour, IPointerClickHandler
    {
        private GridCell _cell;
        private EditorGridManager _gridManager;

        public void Initialize(GridCell cell, EditorGridManager gridManager)
        {
            _cell = cell;
            _gridManager = gridManager;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            if (_cell.HasDot()) return;

            _gridManager.SpawnDotAt(_cell);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}

#endif