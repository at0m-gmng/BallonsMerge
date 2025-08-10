using UniRx;

namespace GameResources.Features.PersistentProgress
{
    public interface IPersistentProgressService : IPersistentListener
    {
        public void InitNewProgress();
        public bool TryLoadData();
        public void SaveData();

    }
    public interface IPersistentListener
    {
        public ReactiveProperty<int> Score { get;}
        public ReactiveProperty<int> ScoreRecord { get;}
    }
}