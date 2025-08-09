using System;
using GameResources.Features.SaveLoadSystem;
using UniRx;

namespace GameResources.Features.PersistentProgress
{
    public sealed class PersistentProgressService : IPersistentProgressService, IDisposable
    {
        public PersistentProgressService(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }
        private readonly ISaveLoadService _saveLoadService;

        public ReactiveProperty<int> Score { get; private set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> ScoreRecord { get; private set; } = new ReactiveProperty<int>();
        private PlayerProgress PlayerProgress
        {
            get => _playerProgress;
            set => _playerProgress = value;
        }
        private PlayerProgress _playerProgress;
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        public void Dispose() => _disposables?.Dispose();

        public void InitNewProgress()
        {
            PlayerProgress = new PlayerProgress();
            Initialize();
        }

        public bool TryLoadData()
        {
            if (_saveLoadService.TryLoadData<PlayerProgress>(out _playerProgress) && PlayerProgress != null)
            {
                Initialize();
                return true;
            }
            return false;
        }
        public void SaveData() => _saveLoadService.SaveData<PlayerProgress>(PlayerProgress);

        private void Initialize()
        {
            Score.Value = PlayerProgress.PlayerScore;
            ScoreRecord.Value = PlayerProgress.PlayerScoreRecord;
            Score.Subscribe(value =>
            {
                PlayerProgress.PlayerScore = value;
                
                if (value > PlayerProgress.PlayerScoreRecord)
                {
                    ScoreRecord.Value = value;
                    PlayerProgress.PlayerScoreRecord = value;
                }
            }).AddTo(_disposables);
        }
    }
}