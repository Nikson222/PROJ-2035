using System;
using System.Collections.Generic;
using _Scripts._Infrastructure.UI;
using _Scripts.Game.Data;
using _Scripts.Game.Progress;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Game
{
    public class LevelSelectionPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _leftArrow;
        [SerializeField] private Button _rightArrow;
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private int _cardsPerPage = 4;

        private LevelDatabase _levelDatabase;
        private UIPanelService _uiPanelService;
        private ProgressManager _progressManager;
        private LevelCard.Pool _levelCardPool;
        private List<LevelCard> _activeCards = new();
        private int _currentPage = 0;
        private Vector3 _startScale;

        [Inject]
        public void Construct(LevelDatabase levelDatabase, ProgressManager progressManager, LevelCard.Pool levelCardPool,
            UIPanelService uiPanelService)
        {
            _startScale = _rectTransform.localScale;
            _levelDatabase = levelDatabase;
            _progressManager = progressManager;
            _levelCardPool = levelCardPool;
            _uiPanelService = uiPanelService;
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(() => Close());
            _leftArrow.onClick.AddListener(OnLeftArrowClicked);
            _rightArrow.onClick.AddListener(OnRightArrowClicked);
            for (int i = 0; i < _cardsPerPage; i++)
            {
                LevelCard card = _levelCardPool.Spawn();
                card.transform.SetParent(_cardContainer, false);
                _activeCards.Add(card);
            }
            UpdateCards();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _rectTransform.localScale = Vector3.zero;
            _rectTransform.DOScale(_startScale, 0.1f).SetEase(Ease.Linear);
        }

        public void Close(Action onClosed = null)
        {
            _rectTransform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

            _uiPanelService.OpenPanel<MenuPanel>();
        }

        private void OnLeftArrowClicked()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                UpdateCards();
            }
        }

        private void OnRightArrowClicked()
        {
            int maxPage = Mathf.CeilToInt(_levelDatabase.Levels.Count / (float)_cardsPerPage) - 1;
            if (_currentPage < maxPage)
            {
                _currentPage++;
                UpdateCards();
            }
        }

        private void UpdateCards()
        {
            int startIndex = _currentPage * _cardsPerPage;
            for (int i = 0; i < _cardsPerPage; i++)
            {
                int levelIndex = startIndex + i;
                if (levelIndex < _levelDatabase.Levels.Count)
                {
                    int stars = _progressManager.GetStarsForLevel(levelIndex);
                    bool unlocked = _progressManager.IsLevelUnlocked(levelIndex);
                    _activeCards[i].Setup(levelIndex, stars, unlocked);
                    _activeCards[i].gameObject.SetActive(true);
                }
                else
                {
                    _activeCards[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
