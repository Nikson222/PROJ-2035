using System;
using System.Collections.Generic;
using _Scripts._Infrastructure.Configs;
using UnityEngine;
using Zenject;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace _Scripts._Infrastructure.UI
{
    public class UIPanelService : IDisposable
    {
        private readonly DiContainer _container;
        private readonly Transform _canvasTransform;
        private readonly Dictionary<Type, GameObject> _registeredPanels = new();
        private readonly Dictionary<Type, IPanel> _activePanels = new();
        private readonly Dictionary<Type, UIPanelConfig.PanelRegistration> _registeredPanelRegistrations = new Dictionary<Type, UIPanelConfig.PanelRegistration>();
        private readonly UIPanelConfig _config;

        public UIPanelService(DiContainer container, [Inject(Id = "UICanvas")] Canvas canvas, UIPanelConfig config)
        {
            _container = container;
            _canvasTransform = canvas.transform;
            _config = config;
            
            UpdateCanvasCamera();
            InitializePanelsForCurrentScene();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateCanvasCamera();
            ClearActivePanels();
            InitializePanelsForCurrentScene();
        }

        private void UpdateCanvasCamera()
        {
            Canvas canvas = _canvasTransform.GetComponent<Canvas>();
            if (canvas == null) return;
            
            Camera newCamera = Camera.main;
            if (newCamera == null) return;
            
            canvas.worldCamera = newCamera;
        }

        private void InitializePanelsForCurrentScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            
            _registeredPanels.Clear();
            _registeredPanelRegistrations.Clear();

            if (_config.PanelRegistrations == null)
            {
                Debug.LogError("[UIPanelService] _config.PanelRegistrations is null!");
                return;
            }
            
            if (_config.PanelRegistrations.Count == 0)
            {
                Debug.LogWarning("[UIPanelService] _config.PanelRegistrations is empty!");
            }

            foreach (var registration in _config.PanelRegistrations)
            {
                if (registration == null)
                {
                    Debug.LogWarning("[UIPanelService] Found null registration in config!");
                    continue;
                }
                
                if (string.IsNullOrEmpty(registration.SceneName))
                {
                    Debug.LogWarning($"[UIPanelService] Registration with empty SceneName: {registration.PanelName}");
                }

                if (!registration.SceneName.Equals(currentSceneName, 
                        StringComparison.OrdinalIgnoreCase)) continue;
                
                if (registration.PanelPrefab == null)
                {
                    Debug.LogError($"[UIPanelService] Registration for {registration.PanelName} has a null PanelPrefab!");
                    continue;
                }
                
                var panelComponent = registration.PanelPrefab.GetComponent<MonoBehaviour>() as IPanel;
                if (panelComponent != null)
                {
                    Type panelType = panelComponent.GetType();
                    _registeredPanels[panelType] = registration.PanelPrefab;
                    _registeredPanelRegistrations[panelType] = registration;
                        
                    var panelGo = _container.InstantiatePrefab(registration.PanelPrefab, _canvasTransform);
                    _container.InjectGameObject(panelGo);
                    IPanel panelInstance = panelGo.GetComponent<IPanel>();
                    if (panelInstance == null)
                    {
                        Debug.LogError($"[UIPanelService] Instantiated panel does not have IPanel component: {registration.PanelName}");
                        continue;
                    }
                    if (registration.IsInitiallyOpen)
                    {
                        panelInstance.Open();
                    }
                    else
                    {
                        panelGo.SetActive(false);
                    }
                    _activePanels[panelType] = panelInstance;
                }
                else
                {
                    Debug.LogWarning($"[UIPanelService] Prefab {registration.PanelPrefab.name} does not implement IPanel.");
                }
            }
        }

        private void ClearActivePanels()
        {
            foreach (var activePanel in _activePanels)
            {
                if (activePanel.Value != null)
                {
                    Object.Destroy((activePanel.Value as MonoBehaviour)?.gameObject);
                }
            }
            _activePanels.Clear();
        }

        public T OpenPanel<T>() where T : MonoBehaviour, IPanel
        {
            Type panelType = typeof(T);
            if (_activePanels.TryGetValue(panelType, out IPanel panel))
            {
                panel.Open();
                return panel as T;
            }

            if (!_registeredPanels.TryGetValue(panelType, out var prefab))
            {
                Debug.LogError($"[UIPanelService] Panel of type {panelType} is not registered for the current scene.");
                return null;
            }

            var panelGo = _container.InstantiatePrefab(prefab, _canvasTransform);
            _container.InjectGameObject(panelGo);
            T newPanel = panelGo.GetComponent<T>();
            if (newPanel == null)
            {
                Debug.LogError($"[UIPanelService] Instantiated panel does not contain component of type {panelType}.");
                return null;
            }
            newPanel.Open();
            _activePanels[panelType] = newPanel;
            return newPanel;
        }

        public void ClosePanel<T>() where T : MonoBehaviour, IPanel
        {
            Type panelType = typeof(T);
            if (_activePanels.TryGetValue(panelType, out IPanel panel))
            {
                panel.Close();
            }
            else
            {
                Debug.LogWarning($"[UIPanelService] Panel of type {panelType} is not active.");
            }
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
