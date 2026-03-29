using _Scripts._Infrastructure;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game
{
    public class LevelCard : MonoBehaviour
    {
        [SerializeField] private Button _levelButton;
        
        [SerializeField] private Text _levelNumberText;
        [SerializeField] private Image _star1;
        [SerializeField] private Image _star2;
        [SerializeField] private Image _star3;
        [SerializeField] private Image _lockImage;

        private int _levelIndex;
        private bool _unlocked;

        private PlayerProfile _playerProfile;
        private AudioService _audioController;
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(PlayerProfile playerProfile, AudioService audioController, SceneLoader sceneLoader)
        {
            _playerProfile = playerProfile;
            _audioController = audioController;
            _sceneLoader = sceneLoader;
            
            _levelButton.onClick.AddListener(OnCardClicked);
        }

        public void Setup(int levelIndex, int stars, bool unlocked)
        {
            _levelIndex = levelIndex;
            _unlocked = unlocked;
            if (_unlocked)
            {
                _levelNumberText.gameObject.SetActive(true);
                _levelNumberText.text = "Level " + (levelIndex + 1);
            }
            else
            {
                _levelNumberText.gameObject.SetActive(false);
            }
            _lockImage.gameObject.SetActive(!_unlocked);
            _star1.enabled = stars >= 1;
            _star2.enabled = stars >= 2;
            _star3.enabled = stars >= 3;
        }

        private void OnCardClicked()
        {
            if (!_unlocked)
                return;
            _playerProfile.SetLevelIndex(_levelIndex);
            _audioController.StopMusic();
            _audioController.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load("LevelScene", () => _audioController.PlayMusic());
        }

        public class Pool : MemoryPool<LevelCard>
        {
            protected override void OnSpawned(LevelCard item)
            {
                item.gameObject.SetActive(true);
            }

            protected override void OnDespawned(LevelCard item)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
