using System;
using _Scripts._Infrastructure.Constants;
using _Scripts.Data;
using _Scripts.Game.Progress;
using Core.Infrastructure.SaveLoad;
using Zenject;

namespace _Scripts._Infrastructure
{
    public class PlayerProfile : ISavable
    {
        private int _currency;
        private int _maxScore;
        private int _lastChosenLevelIndex = 0;

        private ProgressManager _progressManager;

        public int Currency => _currency;
        public int MaxScore => _maxScore;
        public int LastChosenLevelIndex => _lastChosenLevelIndex;

        public string SavePath => SavePathConstants.BalanceDataPath;
        public Type DataType { get; } = typeof(PlayerProfileData);

        public event Action<int> OnCurrencyChanged;

        public void SetLevelIndex(int levelIndex) => _lastChosenLevelIndex = levelIndex;


        [Inject]
        public void Construct(ProgressManager progressManager)
        {
            _progressManager = progressManager;
        }

        public void InitLastChosenLevelIndex() => _lastChosenLevelIndex = _progressManager.GetLastUnlockLevelID();
        
        public void SetMaxScore(int scoreViewScore)
        {
            if(MaxScore < scoreViewScore)
                _maxScore = scoreViewScore;
        }

        public void AddCurrency(int count)
        {
            _currency += count;

            OnCurrencyChanged?.Invoke(_currency);
        }

        public bool RemoveCurrency(int count)
        {
            if (_currency < count)
                return false;

            _currency -= count;

            OnCurrencyChanged?.Invoke(_currency);
            return true;
        }

        public object GetData()
        {
            var balanceData = new PlayerProfileData(Currency, MaxScore);

            return balanceData;
        }

        public void SetData(object data)
        {
            if (data is not PlayerProfileData balanceData) return;
            
            _currency = balanceData.Currency;
            _maxScore = balanceData.MaxScore;
        }

        public void SetInitialData()
        {
            _currency = 0;
            _maxScore = 0;
        }
    }
}