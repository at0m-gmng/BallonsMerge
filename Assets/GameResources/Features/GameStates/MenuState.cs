using GameResources.Features.PersistentProgress;

namespace GameResources.Features.GameStates
{
    using Core;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;

    public sealed class MenuState : IState
    {
        public MenuState(IGameStateMachine gameStateMachine, IUISystem uiSystem, IPersistentListener persistentListener)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
            _persistentListener = persistentListener;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        private readonly IPersistentListener _persistentListener;

        private MenuWindow _menuWindow = default;

        public void Enter()
        {
            Debug.Log($"Enter {nameof(MenuState)}");
            if (_uiSystem.TryGetWindow(UIWindowID.Menu, out _menuWindow))
            {
                _menuWindow.ButtonStart.onClick.AddListener(OnStartButtonClicked);
                _uiSystem.ShowWindow(UIWindowID.Menu);   
            }
            _menuWindow.UpdateView(_persistentListener.ScoreRecord.Value);
        }
        
        public void Exit()
        {
            _menuWindow.ButtonStart.onClick.RemoveListener(OnStartButtonClicked);
            _uiSystem.HideWindow(UIWindowID.Menu);
            Debug.Log($"Exit {nameof(MenuState)}");
        }
        
        private void OnStartButtonClicked() => _gameStateMachine.Enter<GameState>();
    }
}