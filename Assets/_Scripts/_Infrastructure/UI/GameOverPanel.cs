using _Scripts._Infrastructure.Configs;
using _Scripts._Infrastructure.SceneManagement;
using _Scripts._Infrastructure.Services;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System;
using System.Collections;
using _Scripts.Ad;
using _Scripts.Game.Data;
using _Scripts.Game.Progress;

namespace _Scripts._Infrastructure.UI
{
    public class GameOverPanel : MonoBehaviour, IPanel
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _adOfferPanel;
        [SerializeField] private Button _adDeclineButton;
        [SerializeField] private RewardedAdsButton _adAcceptButton;

        [Header("Star References")]
        [SerializeField] private Image[] _inactiveStars;
        [SerializeField] private Image[] _activeStars;

        [Header("Buttons")]
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _nextLevelButton;

        private PlayerProfile _playerProfile;
        private SceneLoader _sceneLoader;
        private AudioService _audioController;
        private LevelDatabase _levelDatabase;
        private ProgressManager _progressManager;

        private Vector3 _startScale;
        private Vector3 _adPanelStartScale;
        private int _earnedStars;

        [Inject]
        private void Construct(
            PlayerProfile playerProfile,
            SceneLoader sceneLoader,
            AudioService audioController,
            LevelDatabase levelDatabase,
            ProgressManager progressManager)
        {
            _playerProfile = playerProfile;
            _sceneLoader = sceneLoader;
            _audioController = audioController;
            _levelDatabase = levelDatabase;
            _progressManager = progressManager;

            _startScale = _rectTransform.localScale;
            _adPanelStartScale = _adOfferPanel.localScale;

            _retryButton.onClick.AddListener(OnClickRetry);
            _menuButton.onClick.AddListener(OnClickMenu);
            _nextLevelButton.onClick.AddListener(OnClickNextLevel);

            _adDeclineButton.onClick.AddListener(OnAdDeclined);
            _adAcceptButton.OnSuccessfulAd += OnAdAccepted;
        }

        private void ResetStars()
        {
            foreach (var star in _activeStars)
                star.gameObject.SetActive(false);
        }

        private IEnumerator AnimateStars()
        {
            for (int i = 0; i < _earnedStars; i++)
            {
                _activeStars[i].gameObject.SetActive(true);
                _activeStars[i].transform.localScale = Vector3.zero;
                _activeStars[i].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(0.35f);
            }

            if (_earnedStars < 3)
            {
                ShowAdOffer();
            }
            else
            {
                SetButtonsInteractable(true);
            }

        }

        private void SetButtonsInteractable(bool state)
        {
            _retryButton.interactable = state;
            _menuButton.interactable = state;
            _nextLevelButton.interactable = state && _earnedStars >= 2;
        }

        private void ShowAdOffer()
        {
            _adOfferPanel.localScale = Vector3.zero;
            _adOfferPanel.gameObject.SetActive(true);
            _adOfferPanel.DOScale(_adPanelStartScale, 0.25f).SetEase(Ease.OutBack);
        }

        private void OnAdAccepted()
        {
            _adOfferPanel.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _adOfferPanel.gameObject.SetActive(false);
                StartCoroutine(AddBonusStar());
            });
        }

        private void OnAdDeclined()
        {
            _adOfferPanel.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _adOfferPanel.gameObject.SetActive(false);
                SetButtonsInteractable(true);
            });
        }

        private IEnumerator AddBonusStar()
        {
            if (_earnedStars >= 3)
                yield break;

            _earnedStars++;
            _activeStars[_earnedStars - 1].gameObject.SetActive(true);
            _activeStars[_earnedStars - 1].transform.localScale = Vector3.zero;
            _activeStars[_earnedStars - 1].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.35f);

            SetButtonsInteractable(true);
            
            _progressManager.SetStarsForLevel(_playerProfile.LastChosenLevelIndex, _earnedStars);
        }

        private void OnClickRetry()
        {
            _audioController.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load("LevelScene", () => _audioController.PlayMusic());
        }

        private void OnClickMenu()
        {
            _audioController.PlaySound(SoundType.ButtonClick);
            _sceneLoader.Load("WhiteMenu", () => _audioController.PlayMusic());
        }

        private void OnClickNextLevel()
        {
            _audioController.PlaySound(SoundType.ButtonClick);
            _playerProfile.SetLevelIndex(_playerProfile.LastChosenLevelIndex + 1);
            _sceneLoader.Load("LevelScene", () => _audioController.PlayMusic());
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _rectTransform.localScale = Vector3.zero;
            _rectTransform.DOScale(_startScale, 0.1f).SetEase(Ease.Linear);

            _earnedStars = _progressManager.GetStarsForLevel(_playerProfile.LastChosenLevelIndex);
            ResetStars();
            StartCoroutine(AnimateStars());

            SetButtonsInteractable(false);
        }

        public void Close(Action onClosed = null)
        {
            _rectTransform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                onClosed?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}