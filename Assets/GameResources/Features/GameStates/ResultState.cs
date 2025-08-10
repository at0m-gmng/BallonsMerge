using GameResources.Features.PersistentProgress;

namespace GameResources.Features.GameStates
{
    using Core;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;

    public sealed class ResultState : IState
    {
        public ResultState(IGameStateMachine gameStateMachine, IUISystem uiSystem, IPersistentProgressService persistentProgress)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
            _persistentProgress = persistentProgress;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        private readonly IPersistentProgressService _persistentProgress;
        
        private ResultWindow _resultWindow = default;

        public void Enter()
        {
            Debug.Log($"Enter {nameof(ResultState)}");
            _uiSystem.TryGetWindow(UIWindowID.Result, out _resultWindow);

            _resultWindow.ButtonRestart.onClick.AddListener(OnRestartButtonClicked);
            _resultWindow.ButtonMenu.onClick.AddListener(OnButtonMenuClicked);
            
            _uiSystem.ShowWindow(UIWindowID.Result);
            _resultWindow.UpdateView(_persistentProgress.Score.Value, _persistentProgress.ScoreRecord.Value);
            _persistentProgress.SaveData();
        }

        public void Exit()
        {
            _uiSystem.HideWindow(UIWindowID.Result);
            Debug.Log($"Exit {nameof(ResultState)}");
        }

        private void OnRestartButtonClicked() => _gameStateMachine.Enter<GameState>();

        private void OnButtonMenuClicked() => _gameStateMachine.Enter<MenuState>();
    }
}