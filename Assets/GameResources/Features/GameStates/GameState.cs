namespace GameResources.Features.GameStates
{
    using Core;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;

    public sealed class GameState : IState
    {
        public GameState(IGameStateMachine gameStateMachine, IUISystem uiSystem)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        
        private GameWindow _gameWindow = default;
        private PauseWindow _pauseWindow = default;
        
        public void Enter()
        {
            Debug.Log($"Enter {nameof(GameState)}");
            _uiSystem.TryGetWindow(UIWindowID.Game, out _gameWindow);
            _uiSystem.TryGetWindow(UIWindowID.Pause, out _pauseWindow);

            _gameWindow.ButtonPause.onClick.AddListener(OnButtonPauseClicked);
            _pauseWindow.ButtonContinue.onClick.AddListener(OnButtonContinueClicked);
            _pauseWindow.ButtonMenu.onClick.AddListener(OnButtonMenuClicked);
            
            _uiSystem.ShowWindow(UIWindowID.Game);
        }

        public void Exit()
        {
            _uiSystem.HideWindow(UIWindowID.Pause);
            _uiSystem.HideWindow(UIWindowID.Game);
            Debug.Log($"Exit {nameof(GameState)}");
        }

        private void OnButtonPauseClicked()
        {
            _uiSystem.ShowWindow(UIWindowID.Pause);
        }

        private void OnButtonContinueClicked()
        {
            _uiSystem.HideWindow(UIWindowID.Pause);
        }
        
        private void OnButtonMenuClicked() => _gameStateMachine.Enter<MenuState>();
    }
}