using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace _Scripts.Game
{
    public class LevelTimerUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text _timerText;
        [SerializeField] private List<Image> _stars;
        [SerializeField] private Sprite _activeStar;
        [SerializeField] private Sprite _inactiveStar;

        private bool _isRunning;
        private int _currentStarsShown;

        public void StartTimer()
        {
            _isRunning = true;
        }

        public void StopTimer()
        {
            _isRunning = false;
        }

        public void UpdateTime(float timeLeft)
        {
            if (!_isRunning)
                return;

            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            _timerText.text = $"{minutes:00}:{seconds:00}";
        }

        public void SetStars(int newCount)
        {
            if (newCount == _currentStarsShown)
                return;

            for (int i = 0; i < _stars.Count; i++)
            {
                bool wasActive = i < _currentStarsShown;
                bool isActive = i < newCount;

                if (wasActive != isActive)
                {
                    _stars[i].sprite = isActive ? _activeStar : _inactiveStar;

                    AnimateStar(_stars[i]);
                }
            }

            _currentStarsShown = newCount;
        }

        private void AnimateStar(Image starImage)
        {
            var tr = starImage.transform;
            tr.DOKill(); 
            tr.localScale = Vector3.one;

            tr.DOScale(1.3f, 0.15f)
              .SetEase(Ease.OutBack)
              .OnComplete(() =>
              {
                  tr.DOScale(1f, 0.15f).SetEase(Ease.InQuad);
              });
        }
    }
}
