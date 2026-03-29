using System;
using UnityEngine;

public class LevelTimer
{
    public event Action<int> OnStarChanged;
    public event Action<float> OnTimeChanged;
    public event Action OnTimeExpired;

    private LevelData _levelData;
    private int _currentStars;
    private float _phaseTimeLeft;

    private bool _isRunning;

    private enum Phase { ThreeStars, TwoStars, OneStar, Expired }
    private Phase _currentPhase;

    public void StartTimer(LevelData levelData)
    {
        _levelData = levelData;

        _currentPhase = Phase.ThreeStars;
        _currentStars = 3;
        _phaseTimeLeft = _levelData.TimeFor3Stars;
        _isRunning = true;

        OnStarChanged?.Invoke(_currentStars);
        OnTimeChanged?.Invoke(_phaseTimeLeft);
    }

    public void Tick()
    {
        if (!_isRunning || _currentPhase == Phase.Expired)
            return;

        _phaseTimeLeft -= Time.deltaTime;
        _phaseTimeLeft = Mathf.Max(_phaseTimeLeft, 0f);

        OnTimeChanged?.Invoke(_phaseTimeLeft);

        if (_phaseTimeLeft <= 0f)
        {
            AdvancePhase();
        }
    }

    private void AdvancePhase()
    {
        switch (_currentPhase)
        {
            case Phase.ThreeStars:
                _currentPhase = Phase.TwoStars;
                _currentStars = 2;
                _phaseTimeLeft = _levelData.TimeFor2Stars - _levelData.TimeFor3Stars;
                break;

            case Phase.TwoStars:
                _currentPhase = Phase.OneStar;
                _currentStars = 1;
                _phaseTimeLeft = _levelData.TimeFor1Star - _levelData.TimeFor2Stars;
                break;

            case Phase.OneStar:
                _currentPhase = Phase.Expired;
                _currentStars = 0;
                _phaseTimeLeft = 0f;
                _isRunning = false;
                OnTimeExpired?.Invoke();
                break;
        }

        OnStarChanged?.Invoke(_currentStars);
        OnTimeChanged?.Invoke(_phaseTimeLeft);
    }

    public int GetCurrentStars() => _currentStars;
    public float GetPhaseTimeLeft() => _phaseTimeLeft;
    public void StopTimer() => _isRunning = false;
}
