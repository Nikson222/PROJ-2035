using System;
using _Scripts._Infrastructure.MyEditorCustoms;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Scripts._Infrastructure
{
    public class InitialBootstrapper : MonoBehaviour
    {
        private AudioService _audioService;
        private SceneLoader _sceneLoader;
        private PlayerProfile _playerProfile;

        [Scene] [SerializeField] private string _menuScene;

        [Inject]
        public void Construct(AudioService audioService, SceneLoader sceneLoader, PlayerProfile playerProfile)
        {
            _audioService = audioService;
            _sceneLoader = sceneLoader;
            _playerProfile = playerProfile;
        }

        private void Awake()
        {
            Application.targetFrameRate = 120;
            LoadMenuScene();
        }
        
        private void LoadMenuScene()
        {
            _sceneLoader.Load(_menuScene, () =>
            {
                _playerProfile.InitLastChosenLevelIndex();
                _audioService.PlayMusic();
            }, 3f);
            Destroy(gameObject);
        }
    }
}