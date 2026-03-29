using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.Services;

namespace _Scripts._Infrastructure.UI
{
    public class SettingsPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _rectTransform;

        [Header("Sound Settings")]
        [SerializeField] private Button _soundIncreaseButton;
        [SerializeField] private Button _soundDecreaseButton;
        [SerializeField] private List<Image> _soundBars;

        [Header("Music Settings")]
        [SerializeField] private Button _musicIncreaseButton;
        [SerializeField] private Button _musicDecreaseButton;
        [SerializeField] private List<Image> _musicBars;

        private Vector3 _startScale;
        private AudioService _audioController;
        private UIPanelService _uiPanelService;

        private int _soundLevel;
        private int _musicLevel;

        [Inject]
        public void Construct(AudioService audioController, UIPanelService uiPanelService)
        {
            _audioController = audioController;
            _uiPanelService = uiPanelService;

            _startScale = _rectTransform.localScale;

            _soundLevel = _soundBars.Count / 2;
            _musicLevel = _musicBars.Count / 2;

            _soundIncreaseButton.onClick.RemoveAllListeners();
            _soundDecreaseButton.onClick.RemoveAllListeners();
            _musicIncreaseButton.onClick.RemoveAllListeners();
            _musicDecreaseButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();

            _soundIncreaseButton.onClick.AddListener(() => ChangeSoundLevel(1));
            _soundDecreaseButton.onClick.AddListener(() => ChangeSoundLevel(-1));

            _musicIncreaseButton.onClick.AddListener(() => ChangeMusicLevel(1));
            _musicDecreaseButton.onClick.AddListener(() => ChangeMusicLevel(-1));

            _closeButton.onClick.AddListener(() => Close());

            ApplySoundVolume();
            ApplyMusicVolume();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _rectTransform.localScale = Vector3.zero;
            _rectTransform.DOScale(_startScale, 0.1f).SetEase(Ease.Linear);
        }

        public void Close(Action onClosed = null)
        {
            _audioController.PlaySound(SoundType.ButtonClick);

            _rectTransform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.SetActive(false);
                onClosed?.Invoke();
                _uiPanelService.OpenPanel<MenuPanel>();
            });
        }

        private void ChangeSoundLevel(int delta)
        {
            int newLevel = Mathf.Clamp(_soundLevel + delta, 0, _soundBars.Count);
            if (newLevel != _soundLevel)
            {
                _soundLevel = newLevel;
                ApplySoundVolume();
            }
        }

        private void ChangeMusicLevel(int delta)
        {
            int newLevel = Mathf.Clamp(_musicLevel + delta, 0, _musicBars.Count);
            if (newLevel != _musicLevel)
            {
                _musicLevel = newLevel;
                ApplyMusicVolume();
            }
        }

        private void ApplySoundVolume()
        {
            float volume = _soundBars.Count == 0 ? 0f : (float)_soundLevel / _soundBars.Count;
            _audioController.SoundVolume = volume;
            _audioController.PlaySound(SoundType.ButtonClick);
            UpdateVisualBars(_soundBars, _soundLevel);
        }

        private void ApplyMusicVolume()
        {
            float volume = _musicBars.Count == 0 ? 0f : (float)_musicLevel / _musicBars.Count;
            _audioController.MusicVolume = volume;
            _audioController.PlaySound(SoundType.ButtonClick);
            UpdateVisualBars(_musicBars, _musicLevel);
        }

        private void UpdateVisualBars(List<Image> bars, int activeCount)
        {
            for (int i = 0; i < bars.Count; i++)
            {
                bars[i].enabled = i < activeCount;
            }
        }
    }
}
