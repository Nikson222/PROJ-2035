using System;
using System.Collections.Generic;
using UnityEngine;
using _Scripts._Infrastructure.MyEditorCustoms;

namespace _Scripts._Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "UIPanelConfig", menuName = "UI/UIPanelConfig")]
    public class UIPanelConfig : ScriptableObject
    {
        [Serializable]
        public class PanelRegistration
        {
            public string PanelName;
            public GameObject PanelPrefab;
            [Scene] public string SceneName;
            public bool IsInitiallyOpen;
        }

        public List<PanelRegistration> PanelRegistrations = new();
    }
}